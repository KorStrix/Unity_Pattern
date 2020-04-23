#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-15 오후 2:48:01
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Unity_Pattern
{
    public struct EffectPlayArg
    {
        public IEffectPlayer pEffectPlayer { get; private set; }
        public bool bCall_FromDeactive { get; private set; }

        public EffectPlayArg(IEffectPlayer pEffectPlayer, bool bCall_FromDeactive = false)
        {
            this.pEffectPlayer = pEffectPlayer; this.bCall_FromDeactive = bCall_FromDeactive;
        }
    }

    public interface IEffectPlayer
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        ObservableCollection<EffectPlayArg> OnFinish_Effect { get; }

        void IEffectPlayer_PlayEffect();
        void IEffectPlayer_PlayEffect_Loop();

        void IEffectPlayer_StopEffect(bool bNotify_OnFinishEffect);
        string strEffectName { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EffectManager : CSingletonNotMonoBase<EffectManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */


        /* protected & private - Field declaration         */

        static Dictionary<string, EffectWrapper> g_mapEffectOriginal = new Dictionary<string, EffectWrapper>();
        static PoolingManager_Component<EffectWrapper> g_pPool = PoolingManager_Component<EffectWrapper>.instance;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public static void Init(Dictionary<string, EffectWrapper> mapEffectOriginal)
        {
            g_mapEffectOriginal = mapEffectOriginal;
        }

        /// <summary>
        /// 이펙트를 실행합니다. <see cref="EffectWrapper"/>을 반환합니다.
        /// </summary>
        /// <param name="strEffectName">플레이할 이펙트의 이름</param>
        /// <param name="OnFinishEffect">이펙트가 끝났을 때 이벤트</param>
        public static EffectWrapper DoPlayEffect(string strEffectName, Vector3 vecPos, System.Action<string> OnFinishEffect = null)
        {
            EffectWrapper pEffect = PlayEffect(strEffectName, OnFinishEffect);
            pEffect.transform.position = vecPos;
            pEffect.IEffectPlayer_PlayEffect();

            return pEffect;
        }

        public static EffectWrapper DoPlayEffect(EffectWrapper pEffect_Origin, Vector3 vecPos, System.Action<string> OnFinishEffect = null)
        {
            if(pEffect_Origin == null)
            {
                Debug.LogError("DoPlayEffect - pEffect_Origin == null");
                return null;
            }

            EffectWrapper pEffect = Pop_EffectWrapper(pEffect_Origin, OnFinishEffect);
            pEffect.transform.position = vecPos;
            pEffect.IEffectPlayer_PlayEffect();

            return pEffect;
        }

        public static EffectWrapper DoPlayEffect_Loop(EffectWrapper pEffect_Origin, Vector3 vecPos)
        {
            if (pEffect_Origin == null)
            {
                Debug.LogError("DoPlayEffect - pEffect_Origin == null");
                return null;
            }

            EffectWrapper pEffect = Pop_EffectWrapper(pEffect_Origin, null);
            pEffect.transform.position = vecPos;
            pEffect.IEffectPlayer_PlayEffect_Loop();

            return pEffect;
        }


        /// <summary>
        /// 이펙트를 실행합니다. <see cref="EffectWrapper"/>을 반환합니다.
        /// </summary>
        /// <param name="strEffectName">플레이할 이펙트의 이름</param>
        /// <param name="OnFinishEffect">이펙트가 끝났을 때 이벤트</param>
        public static EffectWrapper DoPlayEffect(string strEffectName, Transform pTransform, System.Action<string> OnFinishEffect = null)
        {
            EffectWrapper pEffect = PlayEffect(strEffectName, OnFinishEffect);
            Transform pEffectTransform = pEffect.transform;
            pEffectTransform.SetParent(pTransform);
            pEffectTransform.localPosition = Vector3.zero;
            pEffectTransform.localRotation = Quaternion.identity;
            pEffectTransform.localScale = Vector3.one;
            pEffect.IEffectPlayer_PlayEffect();

            return pEffect;
        }

        public static EffectWrapper DoPlayEffect(EffectWrapper pEffect_Origin, Transform pTransform, System.Action<string> OnFinishEffect = null)
        {
            if (pEffect_Origin == null)
            {
                Debug.LogError("DoPlayEffect - pEffect_Origin == null");
                return null;
            }

            EffectWrapper pEffect = Pop_EffectWrapper(pEffect_Origin, OnFinishEffect);
            Transform pEffectTransform = pEffect.transform;
            pEffectTransform.SetParent(pTransform);
            pEffectTransform.localPosition = Vector3.zero;
            pEffectTransform.localRotation = Quaternion.identity;
            pEffectTransform.localScale = Vector3.one;
            pEffect.IEffectPlayer_PlayEffect();

            return pEffect;
        }

        public static EffectWrapper DoPlayEffect_Loop(EffectWrapper pEffect_Origin, Transform pTransform)
        {
            return DoPlayEffect_Loop(pEffect_Origin, pTransform, Vector3.zero);
        }

        public static EffectWrapper DoPlayEffect_Loop(EffectWrapper pEffect_Origin, Transform pTransform, Vector3 vecLocalPos)
        {
            if (pEffect_Origin == null)
            {
                Debug.LogError("DoPlayEffect - pEffect_Origin == null");
                return null;
            }

            EffectWrapper pEffect = Pop_EffectWrapper(pEffect_Origin, null);
            Transform pEffectTransform = pEffect.transform;
            pEffectTransform.SetParent(pTransform);
            pEffectTransform.localPosition = vecLocalPos;
            pEffectTransform.localRotation = Quaternion.identity;
            pEffectTransform.localScale = Vector3.one;
            pEffect.IEffectPlayer_PlayEffect_Loop();

            return pEffect;
        }

        public static void DoStopAllEffect()
        {
            g_pPool.DoPushAll();
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


        }
        protected IEnumerator CoPlayDebug()
        {
            while (true)
            {
                int iInstanceCount = g_pPool.p_iInstanceCount;
                int iUseCount = g_pPool.p_iUseCount;
                gameObject.name = $"{nameof(EffectManager)}_{iUseCount}/{iInstanceCount}개 재생중";

                yield return new WaitForSeconds(0.1f);
            }
        }

        protected override void OnReleaseSingleton()
        {
            base.OnReleaseSingleton();

            g_pPool.DoDestroyAll();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private static EffectWrapper PlayEffect(string strEffectName, Action<string> OnFinishEffect)
        {
            if (g_mapEffectOriginal.ContainsKey(strEffectName) == false)
            {
                Debug.LogError("Error");
            }

            EffectWrapper pEffect = Pop_EffectWrapper(g_mapEffectOriginal[strEffectName], OnFinishEffect);
            return pEffect;
        }

        static EffectWrapper Pop_EffectWrapper(EffectWrapper pEffectWrapper_Origin, Action<string> OnFinishEffect)
        {
            EffectWrapper pEffect = g_pPool.DoPop(pEffectWrapper_Origin);
            pEffect.OnFinish_Effect.DoClear_Observer();
            pEffect.OnFinish_Effect.Subscribe += OnFinish_Effect_Subscribe;
            pEffect.OnFinish_Effect.Subscribe += (Args) => OnFinishEffect?.Invoke(pEffectWrapper_Origin.name);
            pEffect.transform.SetParent(instance.transform);

            return pEffect;
        }

        static private void OnFinish_Effect_Subscribe(EffectPlayArg obj)
        {
            var pEffectPlayer = obj.pEffectPlayer;
            g_pPool.DoPush(pEffectPlayer.gameObject);

#if UNITY_EDITOR
            if(obj.bCall_FromDeactive == false)
                pEffectPlayer.transform.SetParent(instance.transform);
#endif
        }

        #endregion Private
    }
}