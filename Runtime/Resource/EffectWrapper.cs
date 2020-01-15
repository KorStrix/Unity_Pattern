#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-15 오후 4:04:26
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
    public class EffectWrapper : CObjectBase, IEffectPlayer
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public abstract class EffectLogicBase
        {
            abstract public bool bIsPlaying { get; }
            abstract public float fDuration { get; }

            abstract public void DoPlay();
            abstract public void DoStop();
        }

        public class EffectLogic_ParticleSystem : EffectLogicBase
        {
            ParticleSystem _pParticle;

            public override bool bIsPlaying => _pParticle.isPlaying;
            public override float fDuration => _pParticle.main.duration;


            public EffectLogic_ParticleSystem(ParticleSystem pParticle)
            {
                _pParticle = pParticle;
            }

            public override void DoPlay()
            {
                _pParticle.Play();
            }

            public override string ToString()
            {
                return _pParticle.name;
            }

            public override void DoStop()
            {
                _pParticle.Stop();
            }
        }

        /* public - Field declaration            */

        public ObservableCollection<EffectPlayArg> OnFinish_Effect => _OnFinish_Effect;

        public string strEffectName => _pEffectLogic.ToString();

        /* protected & private - Field declaration         */

        public ObservableCollection<EffectPlayArg> _OnFinish_Effect = new ObservableCollection<EffectPlayArg>();

        EffectLogicBase _pEffectLogic;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void IEffectPlayer_PlayEffect()
        {
            IEffectPlayer_StopEffect(false);
            StartCoroutine(nameof(COPlayEffect));
        }

        public void IEffectPlayer_StopEffect(bool bNotify_OnFinishEffect)
        {
            _pEffectLogic.DoStop();

            if (bNotify_OnFinishEffect)
                OnFinish_Effect.DoNotify(new EffectPlayArg(this));
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(pParticleSystem)
            {
                _pEffectLogic = new EffectLogic_ParticleSystem(pParticleSystem);
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator COPlayEffect()
        {
            _pEffectLogic.DoPlay();

            float fDelayTime = 0f;
            while (_pEffectLogic.bIsPlaying)
            {
#if UNITY_EDITOR
                fDelayTime += 0.1f;
                name = $"{_pEffectLogic.ToString()}_{fDelayTime.ToString("F1")}/{_pEffectLogic.fDuration}";
#endif

                yield return new WaitForSeconds(0.1f);
            }

            _OnFinish_Effect.DoNotify(new EffectPlayArg(this));
        }

        #endregion Private

    }
}