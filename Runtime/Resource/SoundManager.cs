#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-14 오후 5:54:08
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    public struct SoundPlayArg
    {
        public ISoundPlayer pSoundPlayer { get; private set; }
        public AudioClip pAudioClip { get; private set; }

        public SoundPlayArg(ISoundPlayer pSoundPlayer, AudioClip pAudioClip)
        {
            this.pSoundPlayer = pSoundPlayer; this.pAudioClip = pAudioClip;
        }
    }

    public interface ISoundPlayer
    {
        ObservableCollection<SoundPlayArg> OnFinish_PlaySound { get; }
        void ISoundPlayer_PlaySound(float fLocalVolume);
        void ISoundPlayer_PlaySound();
        void ISoundPlayer_StopSound(bool bNotify_OnFinishPlaySound);
    }

    /// <summary>
    /// 
    /// </summary>
    public class SoundManager : CSingletonDynamicMonoBase<SoundManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public delegate AudioClip delOnGetSoundClip(string strSoundName);
        static public delOnGetSoundClip OnGetSoundClip;

        /* protected & private - Field declaration         */

        static CPoolingManager_Component<SoundSlot> g_pSlotPool = new CPoolingManager_Component<SoundSlot>();
        static GameObject g_pObject_OriginalSoundSlot;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public static SoundSlot DoPlaySound(string strSoundName, System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound(strSoundName, 1f, OnFinishSound);
        }

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="fLocalVolume">사운드의 볼륨 최종볼륨=(설정 볼륨 * 변수)</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public static SoundSlot DoPlaySound(string strSoundName, float fLocalVolume, System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = g_pSlotPool.DoPop(g_pObject_OriginalSoundSlot);
            pSoundSlot.OnFinish_PlaySound.DoClear_Listener();
            pSoundSlot.OnFinish_PlaySound.Subscribe += OnFinish_PlaySound_Subscribe;
            pSoundSlot.OnFinish_PlaySound.Subscribe += (Args) => OnFinishSound?.Invoke(Args.pAudioClip.name);

            AudioClip pAudioClip = OnGetSoundClip(strSoundName);
            if (pAudioClip == null)
            {
                g_pSlotPool.DoPush(pSoundSlot);
                Debug.LogError($"SoundManager - Not Found Sound {strSoundName}");
                return null;
            }

            pSoundSlot.transform.SetParent(instance.transform);
            pSoundSlot.pAudioSource.clip = pAudioClip;
            pSoundSlot.ISoundPlayer_PlaySound(Calculate_SoundVolume(fLocalVolume));

            return pSoundSlot;
        }

        public static SoundSlot DoPlaySound_3D(string strSoundName, Vector3 vecPos, System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound_3D(strSoundName, 1f, vecPos, OnFinishSound);
        }

        public static SoundSlot DoPlaySound_3D(string strSoundName, float fLocalVolume, Vector3 vecPos, System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = DoPlaySound(strSoundName, fLocalVolume, OnFinishSound);
            pSoundSlot.transform.position = vecPos;

            return pSoundSlot;
        }

        /// <summary>
        /// 음소거 유무입니다
        /// </summary>
        /// <param name="bMute"></param>
        public static void DoMute(bool bMute)
        {

        }

        public static void DoStopAllSound()
        {
            foreach (var pSlot in g_pSlotPool.arrAllObject)
                pSlot.ISoundPlayer_StopSound(false);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AfterSceneLoad()
        {
            g_pObject_OriginalSoundSlot = new GameObject(nameof(SoundSlot) + "_Original");
            g_pObject_OriginalSoundSlot.AddComponent<SoundSlot>();
            g_pObject_OriginalSoundSlot.transform.SetParent(instance.transform);
            g_pObject_OriginalSoundSlot.SetActive(false);

            DontDestroyOnLoad(instance.gameObject);
        }

        protected override IEnumerator OnEnableCoroutine()
        {
#if UNITY_EDITOR
            while(true)
            {
                name = $"SoundManager_{g_pSlotPool.p_iUseCount}/{g_pSlotPool.p_iInstanceCount}개 재생중";

                yield return new WaitForSeconds(0.1f);
            }
#endif

            yield break;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private


        static private void OnFinish_PlaySound_Subscribe(SoundPlayArg obj)
        {
            g_pSlotPool.DoPush((SoundSlot)obj.pSoundPlayer);
        }

        static private float Calculate_SoundVolume(float fLocalVolume)
        {
            return fLocalVolume;
        }


        #endregion Private
    }
}