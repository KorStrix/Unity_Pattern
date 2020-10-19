#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-05-05
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using System;
using System.Collections.Generic;
using Unity_Pattern;
using UnityEngine;

namespace Logic
{
	/// <summary>
	/// 컴포넌트 기반위에서 오브젝트들을 관리하는 로직
	/// <para>예시) ScrollViewItem 관리 등</para>
	/// </summary>
	public class ComponentContainerLogic<TCONTAINED_CLASS>
		where TCONTAINED_CLASS : Component
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/* public - Field declaration               */

        public List<TCONTAINED_CLASS> listActivateItem { get; private set; } = new List<TCONTAINED_CLASS>();

		/* protected & private - Field declaration  */

		static readonly PoolingManager_Component<TCONTAINED_CLASS> _pPool = PoolingManager_Component<TCONTAINED_CLASS>.instance;
		private TCONTAINED_CLASS _pContainObject_Original;
		private Transform _pTransformParents;

		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

        public ComponentContainerLogic(TCONTAINED_CLASS pContainObject_Original)
        {
            if (pContainObject_Original.IsNullComponent())
            {
                Debug.LogError($"{nameof(ComponentContainerLogic<TCONTAINED_CLASS>)} - pContainObject_Original == null");
				return;
			}

			Init(pContainObject_Original, pContainObject_Original.transform.parent);
        }


		public ComponentContainerLogic(TCONTAINED_CLASS pContainObject_Original, Transform pTransformParents)
        {
            Init(pContainObject_Original, pTransformParents);
        }

        public void DoAdd_PoolObject(TCONTAINED_CLASS arrContainedClass)
        {
            _pPool.DoAdd_PoolObject(_pContainObject_Original, arrContainedClass);
        }

        public void DoAdd_PoolObject(IEnumerable<TCONTAINED_CLASS> arrContainedClass)
        {
            _pPool.DoAdd_PoolObject(_pContainObject_Original, arrContainedClass);
        }

        public void DoGenerateObject<TCONTAIN_DATA>(IEnumerable<TCONTAIN_DATA> arrData, Action<TCONTAINED_CLASS, TCONTAIN_DATA> OnInit)
		{
			DoGenerateObject(arrData, OnInit, IsShow_Default);
		}

        public TCONTAINED_CLASS DoGenerateObject_Single()
        {
            return GenerateObject(null);
        }

        public TCONTAINED_CLASS DoGenerateObject_Single(Transform pTransformParents)
        {
            return GenerateObject(pTransformParents);
        }

        public TCONTAINED_CLASS DoGenerateObject_Single<TCONTAIN_DATA>(TCONTAIN_DATA pData, Action<TCONTAINED_CLASS, TCONTAIN_DATA> OnInit)
        {
            return GenerateObject(null, OnInit, IsShow_Default, pData);
        }

        public void DoGenerateObject<TCONTAIN_DATA>(IEnumerable<TCONTAIN_DATA> arrData, Action<TCONTAINED_CLASS, TCONTAIN_DATA> OnInit, Func<TCONTAIN_DATA, bool> OnCheck_IsShowData)
        {
            DoClear();

            arrData.ForEachCustom(pData => GenerateObject(null, OnInit, OnCheck_IsShowData, pData));
		}

        public void DoClear()
        {
            _pPool.DoPushAll();
            listActivateItem.Clear();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void Init(TCONTAINED_CLASS pContainObject_Original, Transform pTransformParents)
        {
            if (pContainObject_Original == null)
            {
                Debug.LogError($"{nameof(ComponentContainerLogic<TCONTAINED_CLASS>)} pContainObject_Original == null");
                return;
            }

            if (pTransformParents == null)
            {
                Debug.LogError($"{nameof(ComponentContainerLogic<TCONTAINED_CLASS>)} pTransformParents == null");
                return;
            }

            _pContainObject_Original = pContainObject_Original;
            _pTransformParents = pTransformParents;
        }

        private TCONTAINED_CLASS GenerateObject<TCONTAIN_DATA>(Transform pTransformParents, Action<TCONTAINED_CLASS, TCONTAIN_DATA> OnInit, Func<TCONTAIN_DATA, bool> OnCheck_IsShowData, TCONTAIN_DATA pData)
        {
            if (OnCheck_IsShowData(pData) == false)
                return null;

            TCONTAINED_CLASS pItemInstance = GenerateObject(pTransformParents);
            OnInit(pItemInstance, pData);

            return pItemInstance;
        }

        private TCONTAINED_CLASS GenerateObject(Transform pTransformParents)
        {
            if (pTransformParents == null)
                pTransformParents = _pTransformParents;

            TCONTAINED_CLASS pItemInstance = _pPool.DoPop(_pContainObject_Original, pTransformParents, false);

            Transform pTransformPropertyDrawer = pItemInstance.transform;
            pTransformPropertyDrawer.SetAsLastSibling();
            pTransformPropertyDrawer.localPosition = Vector3.zero;
            pTransformPropertyDrawer.localScale = Vector3.one;

            listActivateItem.Add(pItemInstance);
            return pItemInstance;
        }

        static bool IsShow_Default<TCONTAIN_DATA>(TCONTAIN_DATA pData) => true;

        #endregion Private
    }
}
