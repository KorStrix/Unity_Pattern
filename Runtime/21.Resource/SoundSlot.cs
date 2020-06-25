#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-14 오후 6:07:19
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class SoundSlot : CObjectBase, ISoundPlayer
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public ObservableCollection<SoundPlayArg> OnFinish_Sound => _OnFinish_PlaySound;

        public string strSoundName { get; private set; }
        public AudioSource pAudioSource { get; private set; }

        /* protected & private - Field declaration         */

        ObservableCollection<SoundPlayArg> _OnFinish_PlaySound = new ObservableCollection<SoundPlayArg>();

        System.Action<SoundPlayArg> _OnFinishUse;
        private float _fSoundScale_0_1;
        bool _bIsLoop;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit(string strSoundName, AudioClip pClip, bool bIsLoop, System.Action<SoundPlayArg> OnFinishUse)
        {
            if(gameObject.activeSelf == false)
                gameObject.SetActive(true);

            this.strSoundName = strSoundName;
            pAudioSource.clip = pClip;
            _bIsLoop = bIsLoop;
            _OnFinishUse = OnFinishUse;
        }

        public void DoSet_AudioVolume(float fVolume_0_1)
        {
            _fSoundScale_0_1 = fVolume_0_1;
            pAudioSource.volume = _fSoundScale_0_1;
        }
        
        public void Event_OnClear()
        {
            OnFinish_Sound.DoClear_Observer();
            _OnFinishUse = null;
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        public void ISoundPlayer_PlaySound(float fLocalVolume)
        {
            DoSet_AudioVolume(fLocalVolume);

            if (gameObject.activeInHierarchy == false)
            {
                Debug.LogError($"{name} {nameof(ISoundPlayer_PlaySound)} gameObject.activeInHierarchy == false", this);
                return;
            }
            ISoundPlayer_PlaySound();
        }

        public void ISoundPlayer_PlaySound()
        {
            ISoundPlayer_StopSound(false);

            if (gameObject.activeInHierarchy == false)
            {
                Debug.LogError($"{name} {nameof(ISoundPlayer_PlaySound)} gameObject.activeInHierarchy == false", this);
                return;
            }
            StartCoroutine(nameof(PlaySound_Coroutine));
        }

        public void ISoundPlayer_StopSound(bool bNotify_OnFinishPlaySound)
        {
            StopCoroutine(nameof(PlaySound_Coroutine));
            pAudioSource.Stop();

            if(bNotify_OnFinishPlaySound)
                ExecuteOnFinishSound();
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            pAudioSource = GetComponent<AudioSource>();
            if (pAudioSource == null)
                pAudioSource = gameObject.AddComponent<AudioSource>();

            pAudioSource.playOnAwake = false;
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            if (bIsQuit_Application)
                return;

            ExecuteOnFinishSound();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator PlaySound_Coroutine()
        {
            pAudioSource.loop = _bIsLoop;
            pAudioSource.Play();

            float fDelayTime = 0f;

            if(_bIsLoop)
            {
                while (true)
                {
#if UNITY_EDITOR
                    fDelayTime += 0.1f;
                    name = $"{pAudioSource.clip.name}/{fDelayTime:F1}/{pAudioSource.clip.length}_IsLoop";
#endif

                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                while (pAudioSource.isPlaying)
                {
#if UNITY_EDITOR
                    fDelayTime += 0.1f;
                    name = $"{pAudioSource.clip.name}/{fDelayTime:F1}/{pAudioSource.clip.length}";
#endif

                    yield return new WaitForSeconds(0.1f);
                }

                gameObject.SetActive(false);
            }
        }

        private void ExecuteOnFinishSound()
        {
            if (_bIsQuit_Application || _bIsEditor_Compiling)
                return;

            _OnFinish_PlaySound.DoNotify(new SoundPlayArg(strSoundName, this, pAudioSource.clip));
            _OnFinishUse?.Invoke(new SoundPlayArg(strSoundName, this, pAudioSource.clip));
        }

        #endregion Private
    }
}