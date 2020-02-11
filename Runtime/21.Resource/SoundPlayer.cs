#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-14 오후 6:06:41
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
    public class SoundPlayer : CObjectBase, ISoundPlayer
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public ObservableCollection<SoundPlayArg> OnFinish_Sound => _OnFinish_PlaySound;

        [Header("What Play Option")]
        public string strPlaySoundName;

        [Header("When Play Option")]
        public bool bPlayOnEnable = true;
        public bool bStopOnDisable = true;

        [Header("How Play Option")]
        public bool bIs3D = true;
        public bool bIsLoop = false;

        [Range(0f, 1f)]
        public float fLocalVolume = 1f;

        public SoundSlot pSoundSlot { get; private set; }

        /* protected & private - Field declaration         */

        public ObservableCollection<SoundPlayArg> _OnFinish_PlaySound = new ObservableCollection<SoundPlayArg>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/


        // ========================================================================== //

        /* protected - Override & Unity API         */

        public void ISoundPlayer_PlaySound()
        {
            if (bIs3D)
                pSoundSlot = SoundManager.instance.DoPlaySound_3D(strPlaySoundName, fLocalVolume, transform.position, bIsLoop);
            else
                pSoundSlot = SoundManager.instance.DoPlaySound(strPlaySoundName, fLocalVolume, bIsLoop);

            pSoundSlot.OnFinish_Sound.Subscribe += _OnFinish_PlaySound.DoNotify;
        }

        public void ISoundPlayer_PlaySound(float fVolume)
        {
            if (bIs3D)
                pSoundSlot = SoundManager.instance.DoPlaySound_3D(strPlaySoundName, fLocalVolume * fVolume, transform.position, bIsLoop);
            else
                pSoundSlot = SoundManager.instance.DoPlaySound(strPlaySoundName, fLocalVolume * fVolume, bIsLoop);

            pSoundSlot.OnFinish_Sound.Subscribe += _OnFinish_PlaySound.DoNotify;
        }

        public void ISoundPlayer_StopSound(bool bNotify_OnFinishPlaySound)
        {
            pSoundSlot?.ISoundPlayer_StopSound(bNotify_OnFinishPlaySound);
        }

        protected override IEnumerator OnEnableCoroutine()
        {
            if (bPlayOnEnable == false)
                yield break;

            while(SoundManager.instance.bIsInit == false)
            {
                yield return null;
            }

            ISoundPlayer_PlaySound();
        }

        private void OnDisable()
        {
            if(bStopOnDisable)
                ISoundPlayer_StopSound(true);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}