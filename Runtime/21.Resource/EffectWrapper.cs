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
    /// Particle System이나 Sprite Animation Effect, Spine Effect 등을 관리하는 이펙트 래퍼클래스입니다.
    /// </summary>
    public class EffectWrapper : CObjectBase, IEffectPlayer
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [SerializeField]
        public abstract class EffectLogicBase
        {
            public abstract bool bIsPlaying { get; }
            public abstract float fDuration { get; }

            public abstract void DoPlay();
            public abstract void DoStop();
        }

        [SerializeField]
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
        Coroutine _pCoroutine_EffectPlay;
        string _strName;

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
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);

            IEffectPlayer_StopEffect(false);
            _pCoroutine_EffectPlay = StartCoroutine(COPlayEffect(false)) ;
        }

        public void IEffectPlayer_PlayEffect_Loop()
        {
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);

            IEffectPlayer_StopEffect(false);
            _pCoroutine_EffectPlay = StartCoroutine(COPlayEffect(true));
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
            _strName = name;
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            if (bIsQuit_Application)
                return;

            _OnFinish_Effect.DoNotify(new EffectPlayArg(this, true));
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator COPlayEffect(bool bIsLoop)
        {
            _pEffectLogic.DoPlay();

#if UNITY_EDITOR
            StartCoroutine(Display_Coroutine(bIsLoop));
#endif

            if(bIsLoop == false)
            {
                yield return new WaitForSeconds(_pEffectLogic.fDuration);

                if (_OnFinish_Effect.iObserverCount != 0)
                    _OnFinish_Effect.DoNotify(new EffectPlayArg(this));
                else
                    gameObject.SetActive(false);
            }

        }

        IEnumerator Display_Coroutine(bool bIsLoop)
        {
            float fDelayTime = 0f;

            if (bIsLoop)
            {
                while (_pEffectLogic.bIsPlaying)
                {
                    fDelayTime += 0.1f;
                    name = $"{_strName}_{fDelayTime.ToString("F1")}/{_pEffectLogic.fDuration}_IsLoop";

                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                while (_pEffectLogic.bIsPlaying)
                {
                    fDelayTime += 0.1f;
                    name = $"{_strName}_{fDelayTime.ToString("F1")}/{_pEffectLogic.fDuration}";

                    yield return new WaitForSeconds(0.1f);
                }
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