﻿#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-14 오후 5:54:08
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Unity_Pattern
{
    public struct SoundPlayArg
    {
        public string strSoundName { get; private set; }
        public ISoundPlayer pSoundPlayer { get; private set; }

        public AudioClip pAudioClip { get; private set; }

        public SoundPlayArg(string strSoundName, ISoundPlayer pSoundPlayer, AudioClip pAudioClip)
        {
            this.strSoundName = strSoundName; this.pSoundPlayer = pSoundPlayer; this.pAudioClip = pAudioClip;
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

        [System.Serializable]
        public class SoundScaleConfig
        {
            public string strCategoryName;
            public float fSoundScale_0_1 = 0.5f;

            public SoundScaleConfig(string strCategoryName)
            {
                this.strCategoryName = strCategoryName;
            }
        }

        [System.Serializable]
        public class SoundConfig
        {
            public bool bIsMute;
            public List<SoundScaleConfig> listSoundConfig = new List<SoundScaleConfig>();
        }

        /* public - Field declaration            */

        public delegate AudioClip delOnGetSoundClip(string strSoundName);

        public bool bIsInit { get; private set; } = false;
        public bool bIsMute { get; private set; } = false;

        /* protected & private - Field declaration         */

        Dictionary<string, List<SoundSlot>> _mapPlayingSoundSlot = new Dictionary<string, List<SoundSlot>>();
        PoolingManager_Component<SoundSlot> _pSlotPool = PoolingManager_Component<SoundSlot>.instance;

        GameObject _pObject_OriginalSoundSlot;
        SoundConfig _pConfig = new SoundConfig();

        delOnGetSoundClip _OnGetSoundClip;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public SoundSlot DoPlaySound(AudioClip pAudioClip, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound(pAudioClip, 1f, false, strSoundCategory, OnFinishSound);
        }

        public SoundSlot DoPlaySound(AudioClip pAudioClip, float fLocalVolume, bool bIsLoop, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = _pSlotPool.DoPop(_pObject_OriginalSoundSlot);
            pSoundSlot.OnFinish_Sound.DoClear_Listener();
            pSoundSlot.OnFinish_Sound.Subscribe += OnFinish_PlaySound_Subscribe;
            pSoundSlot.OnFinish_Sound.Subscribe += (Args) => OnFinishSound?.Invoke(pAudioClip.name);

            if (_OnGetSoundClip == null)
            {
                Debug.LogError("OnGetSoundClip == null");
                return null;
            }

            pSoundSlot.transform.SetParent(instance.transform);
            pSoundSlot.DoInit(pAudioClip.name, pAudioClip, bIsLoop);
            pSoundSlot.ISoundPlayer_PlaySound(Calculate_SoundVolume(strSoundCategory, fLocalVolume));

            if (_mapPlayingSoundSlot.ContainsKey(pAudioClip.name) == false)
                _mapPlayingSoundSlot.Add(pAudioClip.name, new List<SoundSlot>());
            _mapPlayingSoundSlot[pAudioClip.name].Add(pSoundSlot);

            return pSoundSlot;
        }

        public void DoInit(delOnGetSoundClip OnGetSoundClip)
        {
            PlayerPrefsExtension.GetObject(nameof(SoundConfig), ref _pConfig, null);
            _OnGetSoundClip = OnGetSoundClip;

            bIsInit = true;
        }

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public SoundSlot DoPlaySound(string strSoundName, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound(strSoundName, 1f, false, strSoundCategory, OnFinishSound);
        }

        public SoundSlot DoPlaySound_Loop(string strSoundName)
        {
            return DoPlaySound(strSoundName, 1f, true);
        }

        /// <summary>
        /// 사운드를 실행합니다. <see cref="SoundSlot"/>을 반환합니다.
        /// </summary>
        /// <param name="strSoundName">플레이할 사운드의 이름</param>
        /// <param name="fLocalVolume">사운드의 볼륨 최종볼륨=(설정 볼륨 * 변수)</param>
        /// <param name="OnFinishSound">사운드가 끝났을 때 이벤트</param>
        public SoundSlot DoPlaySound(string strSoundName, float fLocalVolume, bool bIsLoop, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = _pSlotPool.DoPop(_pObject_OriginalSoundSlot);
            pSoundSlot.OnFinish_Sound.DoClear_Listener();
            pSoundSlot.OnFinish_Sound.Subscribe_Once += OnFinish_PlaySound_Subscribe;

            if(OnFinishSound != null)
                pSoundSlot.OnFinish_Sound.Subscribe_Once += (Args) => OnFinishSound?.Invoke(strSoundName);

            if(_OnGetSoundClip == null)
            {
                Debug.LogError("OnGetSoundClip == null");
                return null;
            }

            AudioClip pAudioClip = _OnGetSoundClip(strSoundName);
            if (pAudioClip == null)
            {
                _pSlotPool.DoPush(pSoundSlot);
                Debug.LogError($"{nameof(SoundManager)} - Not Found Sound {strSoundName}");
                return null;
            }

            pSoundSlot.transform.SetParent(instance.transform);
            pSoundSlot.DoInit(strSoundName, pAudioClip, bIsLoop);
            pSoundSlot.ISoundPlayer_PlaySound(Calculate_SoundVolume(strSoundCategory, fLocalVolume));

            if (_mapPlayingSoundSlot.ContainsKey(strSoundName) == false)
                _mapPlayingSoundSlot.Add(strSoundName, new List<SoundSlot>());
            _mapPlayingSoundSlot[strSoundName].Add(pSoundSlot);

            return pSoundSlot;
        }

        public SoundSlot DoPlaySound_3D_Loop(string strSoundName, Vector3 vecPos)
        {
            return DoPlaySound_3D(strSoundName, 1f, vecPos, true, null);
        }

        public SoundSlot DoPlaySound_3D(string strSoundName, Vector3 vecPos, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            return DoPlaySound_3D(strSoundName, 1f, vecPos, false, strSoundCategory, OnFinishSound);
        }

        public SoundSlot DoPlaySound_3D(string strSoundName, float fLocalVolume, Vector3 vecPos, bool bIsLoop, string strSoundCategory = "SoundEffect", System.Action<string> OnFinishSound = null)
        {
            SoundSlot pSoundSlot = DoPlaySound(strSoundName, fLocalVolume, bIsLoop, strSoundCategory, OnFinishSound);
            pSoundSlot.transform.position = vecPos;

            return pSoundSlot;
        }

        /// <summary>
        /// 음소거 유무입니다
        /// </summary>
        /// <param name="bMute"></param>
        public void DoMute(bool bMute)
        {
            bIsMute = bMute;

            AudioListener pAudioListener = GameObject.FindObjectOfType<AudioListener>();
            if (pAudioListener != null)
                pAudioListener.enabled = bMute == false;
        }

        public void DoSet_Category_VolumeScale(string strCategory, float fVolume_0_1)
        {
            SoundScaleConfig pConfig = GetSoundScaleConfig(strCategory);
            pConfig.fSoundScale_0_1 = fVolume_0_1;
        }

        public float Get_Category_VolumeScale(string strCategory)
        {
            SoundScaleConfig pConfig = GetSoundScaleConfig(strCategory);
            return pConfig.fSoundScale_0_1;
        }

        public void DoStopSound(string strSoundName)
        {
            if(_mapPlayingSoundSlot.ContainsKey(strSoundName) == false)
            {
                return;
            }

            List<SoundSlot> listSoundSlot = _mapPlayingSoundSlot[strSoundName];
            listSoundSlot[listSoundSlot.Count - 1].ISoundPlayer_StopSound(true);
        }

        public void DoStopAllSound()
        {
            foreach (var pSlot in _pSlotPool.arrAllObject)
            {
                _pSlotPool.DoPush(pSlot);

                if (_mapPlayingSoundSlot.ContainsKey(pSlot.strSoundName))
                    _mapPlayingSoundSlot[pSlot.strSoundName].Remove(pSlot);

                pSlot.OnFinish_Sound.DoRemove_Listener(OnFinish_PlaySound_Subscribe);
                pSlot.ISoundPlayer_StopSound(true);
            }

            foreach (var list in _mapPlayingSoundSlot.Values)
                list.Clear();
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

            _pSlotPool.transform.SetParent(transform);

            _pObject_OriginalSoundSlot = new GameObject(nameof(SoundSlot) + "_Original");
            _pObject_OriginalSoundSlot.AddComponent<SoundSlot>();
            _pObject_OriginalSoundSlot.transform.SetParent(instance.transform);
            _pObject_OriginalSoundSlot.SetActive(false);

            GameObject.DontDestroyOnLoad(instance.gameObject);

            SceneManager.sceneLoaded += OnSceneLoad;

#if UNITY_EDITOR
            pMono.StartCoroutine(CoPlayDebug());
#endif
        }

        private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            DoMute(bIsMute);
        }

        protected IEnumerator CoPlayDebug()
        {
            while (true)
            {
                gameObject.name = $"{nameof(SoundManager)}_{_pSlotPool.p_iUseCount}/{_pSlotPool.p_iInstanceCount}개 재생중";

                yield return new WaitForSeconds(0.1f);
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void OnFinish_PlaySound_Subscribe(SoundPlayArg obj)
        {
            SoundSlot pSlot = (SoundSlot)obj.pSoundPlayer;
            _pSlotPool.DoPush(pSlot);

            if(string.IsNullOrEmpty(obj.strSoundName) == false && _mapPlayingSoundSlot.ContainsKey(obj.strSoundName))
                _mapPlayingSoundSlot[obj.strSoundName].Remove(pSlot);
        }

        private float Calculate_SoundVolume(string strCategory, float fLocalVolume)
        {
            SoundScaleConfig pConfig = GetSoundScaleConfig(strCategory);
            return pConfig.fSoundScale_0_1 * fLocalVolume;
        }

        private SoundScaleConfig GetSoundScaleConfig(string strCategory)
        {
            SoundScaleConfig pScaleConfig = _pConfig.listSoundConfig.Where(pConfig => pConfig.strCategoryName == strCategory).FirstOrDefault();
            if (pScaleConfig == null)
            {
                pScaleConfig = new SoundScaleConfig(strCategory);
                _pConfig.listSoundConfig.Add(pScaleConfig);

                PlayerPrefsExtension.SetObject(nameof(SoundConfig), _pConfig, null);
            }

            return pScaleConfig;
        }

        #endregion Private
    }
}