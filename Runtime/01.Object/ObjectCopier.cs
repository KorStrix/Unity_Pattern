#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-04-01
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
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
	public class ObjectCopier : CObjectBase
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/* public - Field declaration               */

		public GameObject[] arrObjectTarget;
		public int iCopyCount = 3;

		/* protected & private - Field declaration  */

		List<GameObject> _listCopyInstance = new List<GameObject>();

		// ========================================================================== //

		/* public - [Do~Somthing] Function 	        */

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static public void DoCopyAll()
		{
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

			// Unity에선 Obsolute지만
			// 아직 Active/Deactive된 Object 리스트를 한번에 찾는 함수는 이거밖에 없는듯..?
			// https://docs.unity3d.com/ScriptReference/Resources.FindObjectsOfTypeAll.html
			ObjectCopier[] arrAllInstance = FindObjectsOfTypeAll(typeof(ObjectCopier)) as ObjectCopier[];
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

			Debug.Log($"DoCopyAll Start Count : {arrAllInstance.Length}");

			arrAllInstance.ForEachCustom(p => p.DoCopy());
		}

		public void DoCopy()
		{
			Debug.Log($"{name} Copy Start", this);

			for(int i = 0; i < iCopyCount; i++)
			{
				GameObject pObjectTartget = arrObjectTarget.GetRandomItem();
				if (pObjectTartget == null)
				{
					Debug.LogError($"{name} Copy Target Is null", this);
					break;
				}

				GameObject pObjectCopy = GameObject.Instantiate(pObjectTartget, pObjectTartget.transform.parent);
				pObjectCopy.name = $"{pObjectTartget.name}_{_listCopyInstance.Count + 1}";

				_listCopyInstance.Add(pObjectCopy);
			}
		}

		public void DoDestroy_AllInstance()
		{
			_listCopyInstance.ForEach(p => Destroy(p.gameObject));
			_listCopyInstance.Clear();
		}

		// ========================================================================== //

		/* protected - [Override & Unity API]       */


		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		#endregion Private
	}
}