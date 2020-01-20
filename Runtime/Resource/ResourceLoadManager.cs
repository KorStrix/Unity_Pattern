#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-13 오후 9:55:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceLoadManager : CSingletonDynamicMonoBase<ResourceLoadManager>
    {
        /* const & readonly declaration             */

        const string const_EditorPath = "Assets/00.AssetBundlePath";

        /* enum & struct declaration                */

        public abstract class ResourceLoadLogicBase
        {
            abstract public T Load<T>(string strBundleName, string strPath_With_ExtensionName) where T : UnityEngine.Object;
        }

#if UNITY_EDITOR
        public class ResourceLoadLogic_Editor : ResourceLoadLogicBase
        {
            public override T Load<T>(string strBundleName, string strPath_With_ExtensionName)
            {
                string strTotalPath = $"{const_EditorPath}/{strBundleName}/{strPath_With_ExtensionName}";
                T pObject = AssetDatabase.LoadAssetAtPath<T>(strTotalPath);

                if (pObject == null)
                {
                    Debug.LogError($"LoadFail Path : {strTotalPath}");
                    return null;
                }

                return pObject;
            }
        }
#endif

        public class ResourceLoadLogic_StreamingAsset : ResourceLoadLogicBase
        {
            Dictionary<string, AssetBundle> _mapLoadedBundle = new Dictionary<string, AssetBundle>();

            public override T Load<T>(string strBundleName, string strPath_With_ExtensionName)
            {
                if(_mapLoadedBundle.ContainsKey(strBundleName) == false)
                {
                    string strBundlePath = Path.Combine(Application.streamingAssetsPath, strBundleName);
//#if !UNITY_EDITOR && UNITY_ANDROID
//                    strBundlePath = "jar:file://" + Application.dataPath + "!/assets/" + strBundleName;
//#endif

                    Debug.Log("Bundle Path : " + strBundlePath);
                    var pBundleNew = AssetBundle.LoadFromFile(strBundlePath);
                    if (pBundleNew == null)
                    {
                        Debug.LogError($"Failed to load AssetBundle! {strBundleName}");
                        return null;
                    }

                    _mapLoadedBundle.Add(strBundleName, pBundleNew);
                }

                var pBundle = _mapLoadedBundle[strBundleName];
                T pObject = pBundle.LoadAsset<T>(strPath_With_ExtensionName);
                if (pObject == null)
                {
                    Debug.LogError($"Streaming Asset LoadFail  Bundle : {strBundleName} File Name : {strPath_With_ExtensionName}");
                    return null;
                }

                return pObject;
            }
        }

        /* public - Field declaration            */


        /* protected & private - Field declaration         */

#if UNITY_EDITOR
        ResourceLoadLogicBase _pLoadLogic = new ResourceLoadLogic_Editor();
#else
        ResourceLoadLogicBase _pLoadLogic = new ResourceLoadLogic_StreamingAsset();
#endif

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit(ResourceLoadLogicBase pLoadLogic)
        {
            _pLoadLogic = pLoadLogic;
        }

        public T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName) where T : UnityEngine.Object
        {
            return _pLoadLogic.Load<T>(strBundleName, strPath_With_ExtensionName);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(this.gameObject);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}