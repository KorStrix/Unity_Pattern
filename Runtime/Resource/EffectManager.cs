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

        static Dictionary<string, PoolingManager_Component<EffectWrapper>> g_mapEffectPool = new Dictionary<string, PoolingManager_Component<EffectWrapper>>();
        static Dictionary<string, EffectWrapper> g_mapEffectOriginal = new Dictionary<string, EffectWrapper>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 이펙트를 실행합니다. <see cref="EffectWrapper"/>을 반환합니다.
        /// </summary>
        /// <param name="strEffectName">플레이할 이펙트의 이름</param>
        /// <param name="OnFinishEffect">이펙트가 끝났을 때 이벤트</param>
        public static EffectWrapper DoPlayError(string strEffectName, Vector3 vecPos, System.Action<string> OnFinishEffect = null)
        {
            if(g_mapEffectOriginal.ContainsKey(strEffectName) == false)
            {
                Debug.LogError("Error");
            }

            EffectWrapper pEffect = g_mapEffectPool[strEffectName].DoPop(g_mapEffectOriginal[strEffectName]);
            pEffect.OnFinish_Effect.DoClear_Listener();
            pEffect.OnFinish_Effect.Subscribe += OnFinish_Effect_Subscribe;
            pEffect.OnFinish_Effect.Subscribe += (Args) => OnFinishEffect?.Invoke(strEffectName);

            pEffect.transform.SetParent(instance.transform);
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
                int iInstanceCount = 0;
                int iUseCount = 0;
                foreach(var pPool in g_mapEffectPool.Values)
                {
                    iInstanceCount += pPool.p_iInstanceCount;
                    iUseCount += pPool.p_iUseCount;
                }

                gameObject.name = $"{nameof(EffectManager)}_{iUseCount}/{iInstanceCount}개 재생중";

                yield return new WaitForSeconds(0.1f);
            }
        }
        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        static private void OnFinish_Effect_Subscribe(EffectPlayArg obj)
        {
            var pEffectPlayer = obj.pEffectPlayer;
            if (g_mapEffectPool.ContainsKey(pEffectPlayer.strEffectName))
                g_mapEffectPool[pEffectPlayer.strEffectName].DoPush(pEffectPlayer.gameObject);
        }

        #endregion Private
    }
}