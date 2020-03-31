#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-30 오후 1:20:23
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    /// <summary>
    /// <see cref="UnityEngine.Component"/>용 제네릭 풀링 매니져.
    /// <para>사용 방법</para>
    /// <para>풀링에 요청 : <see cref="PoolingManager_Component.DoPop(GameObject pOriginalObject)"/></para>
    /// <para>풀링에 리턴 : 매니져에 관계없이 해당 오브젝트 Disable때 자동 리턴</para>
    /// </summary>
    /// <typeparam name="CLASS_POOL_TARGET">풀링 리턴받을 타입</typeparam>
    public class PoolingManager_Component<CLASS_POOL_TARGET> : PoolingManagerBase<PoolingManager_Component<CLASS_POOL_TARGET>, CLASS_POOL_TARGET>
        where CLASS_POOL_TARGET : Component
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */


        /* protected & private - Field declaration         */

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/


        /// <summary>
        /// 풀링에 있는 오브젝트를 꺼냅니다. 리턴되는 <see cref="GameObject"/> Active는 true입니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">풀링할 오브젝트 원본</param>
        /// <param name="vecWorldPos">배치할 WorldPosition</param>
        public CLASS_POOL_TARGET DoPop(CLASS_POOL_TARGET pObjectCopyTarget, Transform pTransformParents, bool bUseAutoReturn_OnDisable = true)
        {
            var pObject = DoPop(pObjectCopyTarget, pTransformParents.position, bUseAutoReturn_OnDisable);
            pObject.transform.SetParent(pTransformParents);
            pObject.transform.localScale = Vector3.one;
            pObject.transform.localRotation = Quaternion.identity;

            return pObject;
        }

        /// <summary>
        /// 풀링에 있는 오브젝트를 꺼냅니다. 리턴되는 <see cref="GameObject"/> Active는 true입니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">풀링할 오브젝트 원본</param>
        /// <param name="vecWorldPos">배치할 WorldPosition</param>
        public CLASS_POOL_TARGET DoPop(CLASS_POOL_TARGET pObjectCopyTarget, bool bUseAutoReturn_OnDisable = true)
        {
            return DoPop(pObjectCopyTarget, Vector3.zero, bUseAutoReturn_OnDisable);
        }

        /// <summary>
        /// 풀링에 있는 오브젝트를 꺼냅니다. 리턴되는 <see cref="GameObject"/> Active는 true입니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">풀링할 오브젝트 원본</param>
        /// <param name="vecWorldPos">배치할 WorldPosition</param>
        public CLASS_POOL_TARGET DoPop(CLASS_POOL_TARGET pObjectCopyTarget, Vector3 vecWorldPos, bool bUseAutoReturn_OnDisable = true)
        {
            CLASS_POOL_TARGET pUnUsed = base.DoPop(pObjectCopyTarget);
            pUnUsed.transform.position = vecWorldPos;

            if(bUseAutoReturn_OnDisable)
            {
                EventTrigger_OnDisable pEventTrigger_AutoReturn = pUnUsed.GetComponent<EventTrigger_OnDisable>();
                pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;
                pEventTrigger_AutoReturn.p_Event_OnDisable += DoPush;
            }

            return pUnUsed;
        }

        /// <summary>
        /// 풀링에 있는 오브젝트를 꺼냅니다. 리턴되는 <see cref="GameObject"/> Active는 true입니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">풀링할 오브젝트 원본</param>
        public CLASS_POOL_TARGET DoPop(GameObject pObjectCopyTarget, bool bUseAutoReturn_OnDisable = true)
        {
            if(pObjectCopyTarget == null)
            {
                Debug.LogError("pObjectCopyTarget == null");
                return null;
            }

            return DoPop(pObjectCopyTarget.GetComponent<CLASS_POOL_TARGET>(), Vector3.zero, bUseAutoReturn_OnDisable);
        }

        public override void DoDestroyAll()
        {
            List<CLASS_POOL_TARGET> listDestroyKey = new List<CLASS_POOL_TARGET>();
            listDestroyKey.AddRange(_mapAllInstance.Keys);

            foreach (var pObject in listDestroyKey)
            {
                if (pObject != null)
                    GameObject.DestroyImmediate(pObject.gameObject);
            }

            base.DoDestroyAll();
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override CLASS_POOL_TARGET OnCreateClass_WhenEmptyPool(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
        {
            GameObject pObjectUnUsed = GameObject.Instantiate(pObjectCopyTarget.gameObject);
            pObjectUnUsed.name = string.Format("{0}_{1}", pObjectCopyTarget.name, _mapUnUsed[iID].Count + _mapUsed[iID].Count);
            pObjectUnUsed.transform.SetParent(transform);

            EventTrigger_OnDisable pEventTrigger = pObjectUnUsed.gameObject.AddComponent<EventTrigger_OnDisable>();
            pEventTrigger.p_Event_OnDestroy += Event_RemovePoolObject;


            CLASS_POOL_TARGET pComponentUnUsed = pObjectUnUsed.GetComponent<CLASS_POOL_TARGET>();
            if (pComponentUnUsed == null)
                Debug.LogError("풀링 매니져 에러 - pComponentNew == null, Origin Object : " + pObjectCopyTarget.name, pObjectCopyTarget);

            return pComponentUnUsed;
        }

        protected override void OnPopObject(CLASS_POOL_TARGET pClassType)
        {
            if (pClassType.gameObject != null && pClassType.gameObject.activeSelf == false)
                pClassType.gameObject.SetActive(true);
        }

        protected override void OnPushObject(CLASS_POOL_TARGET pClassType)
        {
            if (pClassType.gameObject != null && pClassType.gameObject.activeSelf)
                pClassType.gameObject.SetActive(false);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}