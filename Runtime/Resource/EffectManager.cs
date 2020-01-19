﻿#region Header
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

        public EffectPlayArg(IEffectPlayer pEffectPlayer)
        {
            this.pEffectPlayer = pEffectPlayer;
        }
    }

    public interface IEffectPlayer
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        ObservableCollection<EffectPlayArg> OnFinish_Effect { get; }

        void IEffectPlayer_PlayEffect();
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

        static public void Init(Dictionary<string, EffectWrapper> mapEffectOriginal)
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

        /// <summary>
        /// 이펙트를 실행합니다. <see cref="EffectWrapper"/>을 반환합니다.
        /// </summary>
        /// <param name="strEffectName">플레이할 이펙트의 이름</param>
        /// <param name="OnFinishEffect">이펙트가 끝났을 때 이벤트</param>
        public static EffectWrapper DoPlayEffect(string strEffectName, Transform pTransform, System.Action<string> OnFinishEffect = null)
        {
            EffectWrapper pEffect = PlayEffect(strEffectName, OnFinishEffect);
            pEffect.transform.SetParent(pTransform);
            pEffect.transform.localPosition = Vector3.zero;
            pEffect.transform.localRotation = Quaternion.identity;
            pEffect.transform.localScale = Vector3.one;
            pEffect.IEffectPlayer_PlayEffect();

            return pEffect;
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
        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private static EffectWrapper PlayEffect(string strEffectName, Action<string> OnFinishEffect)
        {
            if (g_mapEffectOriginal.ContainsKey(strEffectName) == false)
            {
                Debug.LogError("Error");
            }

            EffectWrapper pEffect = g_pPool.DoPop(g_mapEffectOriginal[strEffectName]);
            pEffect.OnFinish_Effect.DoClear_Listener();
            pEffect.OnFinish_Effect.Subscribe += OnFinish_Effect_Subscribe;
            pEffect.OnFinish_Effect.Subscribe += (Args) => OnFinishEffect?.Invoke(strEffectName);

            pEffect.transform.SetParent(instance.transform);
            return pEffect;
        }

        static private void OnFinish_Effect_Subscribe(EffectPlayArg obj)
        {
            var pEffectPlayer = obj.pEffectPlayer;
            g_pPool.DoPush(pEffectPlayer.gameObject);
        }

        #endregion Private
    }
}