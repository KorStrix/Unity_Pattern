#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-04-01
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

		public GameObject pObjectTarget;
		public int iCopyCount = 3;

		/* protected & private - Field declaration  */

		List<GameObject> _listCopyInstance = new List<GameObject>();

		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void DoCopyAll()
		{
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

			// Unity에선 Obsolute지만
			// 아직 Active/Deactive된 Object 리스트를 한번에 찾는 함수는 이거밖에 없는듯..?
			// https://docs.unity3d.com/ScriptReference/Resources.FindObjectsOfTypeAll.html

			// 근데 프리팹을 찾음;
			ObjectCopier[] arrAllInstance = FindObjectsOfTypeAll(typeof(ObjectCopier)) as ObjectCopier[];
			// 프리팹을 거르는 Where 코드
			// https://answers.unity.com/questions/218429/how-to-know-if-a-gameobject-is-a-prefab.html
			// arrAllInstance.Where(p => p.gameObject.scene.name != null).

			arrAllInstance.
#if UNITY_EDITOR
				Where(p => UnityEditor.PrefabUtility.GetPrefabType(p) != PrefabType.Prefab).
#endif
				Where(p => p.enabled).
				ForEachCustom(p => p.DoCopy());
		}

		public void DoCopy()
		{
			Debug.Log($"{name} Copy Start", this);
			DoDestroy_AllInstance();

			Transform pTransformParent = pObjectTarget.transform.parent;
			if (pTransformParent == null)
				pTransformParent = transform;
			
			for (int i = 0; i < iCopyCount; i++)
			{
				if (pObjectTarget == null)
				{
					Debug.LogError($"{name} Copy Target Is null", this);
					break;
				}

				GameObject pObjectCopy = GameObject.Instantiate(pObjectTarget);
				pObjectCopy.name = $"{pObjectTarget.name}_{_listCopyInstance.Count + 1}";

				Transform pTransformCopy = pObjectCopy.transform;
				pTransformCopy.SetParent(pTransformParent);
				pTransformCopy.localPosition = Vector3.zero;
				pTransformCopy.localScale = Vector3.one;
				pTransformCopy.localRotation = Quaternion.identity;

				_listCopyInstance.Add(pObjectCopy);
			}
		}

		public void DoDestroy_AllInstance()
		{
			_listCopyInstance.ForEach(p => Destroy(p.gameObject));
			_listCopyInstance.Clear();
		}

		public void DoDestroyImmediate_AllInstance()
		{
			_listCopyInstance.ForEach(p => DestroyImmediate(p.gameObject));
			_listCopyInstance.Clear();
		}

		// ========================================================================== //

		/* protected - [Override & Unity API]       */


		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		#endregion Private
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(ObjectCopier))]
	public class ObjectCopier_Inspector : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			ObjectCopier pTarget = target as ObjectCopier;

			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Copy"))
				{
					pTarget.DoCopy();
				}

				if (GUILayout.Button("Clear"))
				{
					pTarget.DoDestroyImmediate_AllInstance();
				}
			}
			GUILayout.EndHorizontal();
		}
	}
#endif
}
