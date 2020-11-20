#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-10-05
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity_Pattern
{
    /// <summary>
    /// 이 오브젝트의 자식을 n개만큼 카피합니다.
    /// </summary>
    public class CopyChildrenObject : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public Dictionary<string, IReadOnlyList<GameObject>> mapCopyObjectList => _mapCopyObjectList;

        public int iChildCopyCount = 10;
        public string strSuffix = "_Copy";

        /* protected & private - Field declaration  */

        private Dictionary<string, IReadOnlyList<GameObject>> _mapCopyObjectList = new Dictionary<string, IReadOnlyList<GameObject>>();

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInit_And_StartCopy()
        {
            _mapCopyObjectList.Clear();

            Transform pTransform_CopyRoot = GetOrCreate_CopyRoot();
            Destroy_CopyTargetChildren(pTransform_CopyRoot);

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject pObjectCopyTargetChild = transform.GetChild(i).gameObject;
                string strOriginChildName = pObjectCopyTargetChild.name;
                if (_mapCopyObjectList.ContainsKey_Safe(strOriginChildName, Debug.LogError))
                    continue;

                List<GameObject> listCopyChildren = new List<GameObject>();
                _mapCopyObjectList.Add(strOriginChildName, listCopyChildren);

                for (int j = 0; j < iChildCopyCount; j++)
                {
                    GameObject pObjectCopyChild = GameObject.Instantiate(pObjectCopyTargetChild, pTransform_CopyRoot);
                    pObjectCopyChild.name = $"{strOriginChildName}_{j + 1}";

                    listCopyChildren.Add(pObjectCopyChild);
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(transform.root);
#endif

            Debug.Log("Copy Done", pTransform_CopyRoot);
        }

        public void DoReset()
        {
            _mapCopyObjectList.Clear();

            Transform pTransform_CopyRoot = GetOrCreate_CopyRoot();
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject pCopyTargetChild = transform.GetChild(i).gameObject;
                string strOriginChildName = pCopyTargetChild.name;
                if (_mapCopyObjectList.ContainsKey_Safe(strOriginChildName, Debug.LogError))
                    continue;

                List<GameObject> listCopyChildren = new List<GameObject>();
                _mapCopyObjectList.Add(strOriginChildName, listCopyChildren);

                for (int j = 0; j < pTransform_CopyRoot.childCount; j++)
                {
                    GameObject pCopiedChild = pTransform_CopyRoot.GetChild(j).gameObject;
                    if (pCopiedChild.name.Contains(strOriginChildName + "_"))
                        listCopyChildren.Add(pCopiedChild);
                }
            }
        }

        public IReadOnlyList<T> GetCopyComponentList<T>(string strObjectName)
            where T : Component
        {
            if (_mapCopyObjectList.TryGetValue_Custom(strObjectName, out var list, Debug.LogError) == false)
                return new List<T>();

            List<T> listComponent = new List<T>();
            list.ForEachCustom(p => listComponent.Add(p.GetComponent<T>()));

            return listComponent;
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            DoReset();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private Transform GetOrCreate_CopyRoot()
        {
            string strCopyRootObjectName = name + strSuffix;
            Transform pTransform_CopyRoot = transform.parent.Find(strCopyRootObjectName);
            if (pTransform_CopyRoot == null)
            {
                pTransform_CopyRoot = new GameObject(strCopyRootObjectName).transform;
                pTransform_CopyRoot.parent = transform.parent;

                if (GetComponent<RectTransform>() != null)
                    pTransform_CopyRoot = pTransform_CopyRoot.gameObject.AddComponent<RectTransform>();
            }

            pTransform_CopyRoot.localScale = transform.localScale;
            pTransform_CopyRoot.localRotation = transform.localRotation;
            pTransform_CopyRoot.localPosition = transform.localPosition;
            pTransform_CopyRoot.SetSiblingIndex(transform.GetSiblingIndex() + 1);

            return pTransform_CopyRoot;
        }

        private static void Destroy_CopyTargetChildren(Transform pTransform_CopyRoot)
        {
            while(pTransform_CopyRoot.childCount != 0)
                DestroyImmediate(pTransform_CopyRoot.GetChild(0).gameObject);
        }

        #endregion Private
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CopyChildrenObject))]
    public class CopyChildrenObject_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CopyChildrenObject pTarget = target as CopyChildrenObject;

            if (GUILayout.Button("Copy Children"))
            {
                pTarget.DoInit_And_StartCopy();
            }
        }
    }
#endif
}
