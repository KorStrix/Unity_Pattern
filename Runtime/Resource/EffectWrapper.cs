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
using UnityEditor;

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


            public EffectLogic_ParticleSystem(ParticleSystem pParticle, string strSortingLayer)
            {
                _pParticle = pParticle;

                Renderer[] arrRenderer = _pParticle.GetComponentsInChildren<Renderer>();
                for(int i = 0; i < arrRenderer.Length; i++)
                {
                    arrRenderer[i].sortingLayerName = strSortingLayer;
                }
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

        [SortingLayerAttribute]
        public string strSortLayerID;

        /* protected & private - Field declaration         */

        public ObservableCollection<EffectPlayArg> _OnFinish_Effect = new ObservableCollection<EffectPlayArg>();

        EffectLogicBase _pEffectLogic;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit()
        {
            ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();
            if (pParticleSystem)
            {
                _pEffectLogic = new EffectLogic_ParticleSystem(pParticleSystem, strSortLayerID);
            }
        }

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

            DoInit();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator COPlayEffect()
        {
            _pEffectLogic.DoPlay();

#if UNITY_EDITOR
            StartCoroutine(Display_Coroutine());
#endif

            yield return new WaitForSeconds(_pEffectLogic.fDuration);
            _OnFinish_Effect.DoNotify(new EffectPlayArg(this));
        }

        IEnumerator Display_Coroutine()
        {
            string strName = name;
            float fDelayTime = 0f;
            while (_pEffectLogic.bIsPlaying)
            {
                fDelayTime += 0.1f;
                name = $"{strName}_{fDelayTime.ToString("F1")}/{_pEffectLogic.fDuration}";

                yield return new WaitForSeconds(0.1f);
            }

        }

        #endregion Private

    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(EffectWrapper))]
    public class EffectWrapper_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EffectWrapper pTarget = target as EffectWrapper;
            pTarget.DoInit();
        }
    }
#endif
}