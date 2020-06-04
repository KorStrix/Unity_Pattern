#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-05-05
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity_Pattern;

namespace Logic
{
	/// <summary>
	/// 컴포넌트 기반위에서 오브젝트들을 관리하는 로직
	/// <para>예시) ScrollViewItem 관리 등</para>
	/// </summary>
	public class ComponentContainerLogic<CONTAINED_CLASS>
		where CONTAINED_CLASS : Component
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/* public - Field declaration               */

        public List<CONTAINED_CLASS> listActivateItem { get; private set; } = new List<CONTAINED_CLASS>();

		/* protected & private - Field declaration  */

		static readonly PoolingManager_Component<CONTAINED_CLASS> _pPool = PoolingManager_Component<CONTAINED_CLASS>.instance;
		private readonly CONTAINED_CLASS _pContainObject_Original;
		private readonly Transform _pTransformParents;

		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

		public ComponentContainerLogic(CONTAINED_CLASS pContainObject_Original, Transform pTransformParents)
		{
            if (pContainObject_Original == null)
            {
                Debug.LogError($"{nameof(ComponentContainerLogic<CONTAINED_CLASS>)} pContainObject_Original == null");
				return;
            }

            if (pTransformParents == null)
            {
                Debug.LogError($"{nameof(ComponentContainerLogic<CONTAINED_CLASS>)} pTransformParents == null");
                return;
            }

			_pContainObject_Original = pContainObject_Original;
			_pTransformParents = pTransformParents;
		}
		
		public void DoGenerateObject<CONTAIN_DATA>(IEnumerable<CONTAIN_DATA> arrData, System.Action<CONTAINED_CLASS, CONTAIN_DATA> OnInit)
		{
			DoGenerateObject(arrData, OnInit, IsShow_Default);
		}
		
		public void DoGenerateObject<CONTAIN_DATA>(IEnumerable<CONTAIN_DATA> arrData, System.Action<CONTAINED_CLASS, CONTAIN_DATA> OnInit, System.Func<CONTAIN_DATA, bool> OnCheck_IsShowData)
		{
			_pPool.DoPushAll();

			foreach (var pData in arrData)
			{
				if (OnCheck_IsShowData(pData) == false)
					continue;
				
				CONTAINED_CLASS pItemInstance = _pPool.DoPop(_pContainObject_Original, false);

				Transform pTransformPropertyDrawer = pItemInstance.transform;
				pTransformPropertyDrawer.SetParent(_pTransformParents);
				pTransformPropertyDrawer.SetAsLastSibling();
                pTransformPropertyDrawer.localPosition = Vector3.zero;
				pTransformPropertyDrawer.localScale = Vector3.one;

                listActivateItem.Add(pItemInstance);
				OnInit(pItemInstance, pData);
            }
		}

		static bool IsShow_Default<CONTAIN_DATA>(CONTAIN_DATA pData)
		{
			return true;
		}
		
		// ========================================================================== //

		/* protected - [Override & Unity API]       */


		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		#endregion Private
	}
}