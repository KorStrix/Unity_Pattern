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

namespace Unity_Pattern
{
	/// <summary>
	/// 
	/// </summary>
	public class ComponentContainerLogic<CONTAINED_CLASS>
		where CONTAINED_CLASS : Component
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/* public - Field declaration               */


		/* protected & private - Field declaration  */

		static readonly PoolingManager_Component<CONTAINED_CLASS> _pPool = PoolingManager_Component<CONTAINED_CLASS>.instance;
		private CONTAINED_CLASS _pContainObject_Original;
		private Transform _pTransformParents;
		
		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

		public ComponentContainerLogic(CONTAINED_CLASS pContainObject_Original, Transform pTransformParents)
		{
			_pContainObject_Original = pContainObject_Original;
			_pTransformParents = pTransformParents;
		}
		
		public void DoGenerateObject<CONTAIN_DATA>(IEnumerable<CONTAIN_DATA> arrData, System.Action<CONTAINED_CLASS, CONTAIN_DATA> OnInit)
		{
			DoGenerateObject(arrData, OnInit, IsShow_Default<CONTAIN_DATA>);
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
				pTransformPropertyDrawer.localScale = Vector3.one;
			
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