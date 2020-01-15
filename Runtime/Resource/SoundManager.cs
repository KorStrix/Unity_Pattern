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
        ObservableCollection<SoundPlayArg> OnFinish_Sound { get; }
        void ISoundPlayer_PlaySound(float fLocalVolume);
        void ISoundPlayer_PlaySound();
        void ISoundPlayer_StopSound(bool bNotify_OnFinishPlaySound);
    }

    /// <summary>
    /// 
    /// </summary>
    public class SoundManager : CSingletonNotMonoBase<SoundManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public delegate AudioClip delOnGetSoundClip(string strSoundName);
        static public delOnGetSoundClip OnGetSoundClip;

        /* protected & private - Field declaration         */

        PoolingManager_Component<SoundSlot> g_pSlotPool = new PoolingManager_Component<SoundSlot>();
        GameObject g_pObject_OriginalSoundSlot;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public SoundSlot DoPlaySound(string strSoundName, System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound(strSoundName, 1f, OnFinishSound);
        }

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="fLocalVolume">사운드의 볼륨 최종볼륨=(설정 볼륨 * 변수)</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public SoundSlot DoPlaySound(string strSoundName, float fLocalVolume, System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = g_pSlotPool.DoPop(g_pObject_OriginalSoundSlot);
            pSoundSlot.OnFinish_Sound.DoClear_Listener();
            pSoundSlot.OnFinish_Sound.Subscribe += OnFinish_PlaySound_Subscribe;
            pSoundSlot.OnFinish_Sound.Subscribe += (Args) => OnFinishSound?.Invoke(strSoundName);

            AudioClip pAudioClip = OnGetSoundClip(strSoundName);
            if (pAudioClip == null)
            {
                g_pSlotPool.DoPush(pSoundSlot);
                Debug.LogError($"{nameof(SoundManager)} - Not Found Sound {strSoundName}");
                return null;
            }

            pSoundSlot.transform.SetParent(instance.transform);
            pSoundSlot.pAudioSource.clip = pAudioClip;
            pSoundSlot.ISoundPlayer_PlaySound(Calculate_SoundVolume(fLocalVolume));

            return pSoundSlot;
        }

        public SoundSlot DoPlaySound_3D(string strSoundName, Vector3 vecPos, System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound_3D(strSoundName, 1f, vecPos, OnFinishSound);
        }

        public SoundSlot DoPlaySound_3D(string strSoundName, float fLocalVolume, Vector3 vecPos, System.Action<string> OnFinishSound = null)
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

        public void DoStopAllSound()
        {
            foreach (var pSlot in g_pSlotPool.arrAllObject)
                pSlot.ISoundPlayer_StopSound(false);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnMakeSingleton(out bool bIsGenearteGameObject_Default_Is_False)
        {
            base.OnMakeSingleton(out bIsGenearteGameObject_Default_Is_False);

            bIsGenearteGameObject_Default_Is_False = true;
        }

        protected override void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono)
        {
            base.OnMakeGameObject(pObject, pMono);

            g_pSlotPool.transform.SetParent(transform);

            g_pObject_OriginalSoundSlot = new GameObject(nameof(SoundSlot) + "_Original");
            g_pObject_OriginalSoundSlot.AddComponent<SoundSlot>();
            g_pObject_OriginalSoundSlot.transform.SetParent(instance.transform);
            g_pObject_OriginalSoundSlot.SetActive(false);

            GameObject.DontDestroyOnLoad(instance.gameObject);

#if UNITY_EDITOR
            pMono.StartCoroutine(CoPlayDebug());
#endif
        }

        protected IEnumerator CoPlayDebug()
        {
            while (true)
            {
                gameObject.name = $"{nameof(SoundManager)}_{g_pSlotPool.p_iUseCount}/{g_pSlotPool.p_iInstanceCount}개 재생중";

                yield return new WaitForSeconds(0.1f);
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void OnFinish_PlaySound_Subscribe(SoundPlayArg obj)
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