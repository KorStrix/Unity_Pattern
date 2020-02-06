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
        public AudioSource pAudioSource { get; private set; }

        /* protected & private - Field declaration         */

        ObservableCollection<SoundPlayArg> _OnFinish_PlaySound = new ObservableCollection<SoundPlayArg>();

        string _strSoundName;
        bool _bIsLoop;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit(string strSoundName, AudioClip pClip, bool bIsLoop)
        {
            if(gameObject.activeSelf == false)
                gameObject.SetActive(true);

            _strSoundName = strSoundName;
            pAudioSource.clip = pClip;
            _bIsLoop = bIsLoop;
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        public void ISoundPlayer_PlaySound(float fLocalVolume)
        {
            pAudioSource.volume = fLocalVolume;

            ISoundPlayer_PlaySound();
        }

        public void ISoundPlayer_PlaySound()
        {
            ISoundPlayer_StopSound(false);
            StartCoroutine(nameof(COPlaySound));
        }

        public void ISoundPlayer_StopSound(bool bNotify_OnFinishPlaySound)
        {
            StopCoroutine(nameof(COPlaySound));
            pAudioSource.Stop();

            if(bNotify_OnFinishPlaySound)
                _OnFinish_PlaySound.DoNotify(new SoundPlayArg(_strSoundName, this, pAudioSource.clip));
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

            _OnFinish_PlaySound.DoNotify(new SoundPlayArg(_strSoundName, this, pAudioSource.clip));
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator COPlaySound()
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
                    name = $"{pAudioSource.clip.name}_{fDelayTime.ToString("F1")}/{pAudioSource.clip.length}_IsLoop";
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
                    name = $"{pAudioSource.clip.name}_{fDelayTime.ToString("F1")}/{pAudioSource.clip.length}";
#endif

                    yield return new WaitForSeconds(0.1f);
                }

                _OnFinish_PlaySound.DoNotify(new SoundPlayArg(_strSoundName, this, pAudioSource.clip));
            }
        }

        #endregion Private


    }
}