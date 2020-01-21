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
    public class BundleLoadManager : CSingletonDynamicMonoBase<BundleLoadManager>
    {
        /* const & readonly declaration             */

        const string const_EditorPath = "Assets/00.AssetBundlePath";

        /* enum & struct declaration                */

        public class BundleWrapper
        {
            public string strBundleName { get; private set; }
            public AssetBundle pBundle { get; private set; }
            public AsyncOperation pAsyncOperation { get; private set; }

            public BundleWrapper(string strBundleName, AsyncOperation pAsyncOperation)
            {
                this.strBundleName = strBundleName; this.pAsyncOperation = pAsyncOperation;
            }

            public void DoSetBundle(AssetBundle pBundle)
            {
                this.pBundle = pBundle;
            }
        }

        public abstract class ResourceLoadLogicBase
        {
            abstract public IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle);

            abstract public T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError) where T : UnityEngine.Object;
        }

#if UNITY_EDITOR
        public class ResourceLoadLogic_Editor : ResourceLoadLogicBase
        {
            public override IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle)
            {
                OnLoadBundle(strBundleName, true);
                yield break;
            }

            public override T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError)
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
            Dictionary<string, BundleWrapper> _mapLoadedBundle = new Dictionary<string, BundleWrapper>();

            public override IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle)
            {
                bool bLoaded = false;
                if (_mapLoadedBundle.ContainsKey(strBundleName))
                {
                    var pAsyncExist = _mapLoadedBundle[strBundleName].pAsyncOperation;
                    if (pAsyncExist.isDone)
                    {
                        OnLoadBundle(strBundleName, true);
                    }
                    else
                    {
                        // yield return pAsyncExist; 다른 코루틴에서 yield 탄다고 에러 뱉음
                        while (pAsyncExist.isDone == false)
                        {
                            yield return null;
                        }

                        bLoaded = true;
                    }
                }

                if(bLoaded == false)
                {
                    var pAsync = AssetBundle.LoadFromFileAsync(GetBundlePath(strBundleName));
                    BundleWrapper pBundleWrapper = new BundleWrapper(strBundleName, pAsync);
                    _mapLoadedBundle.Add(strBundleName, pBundleWrapper);

                    yield return pAsync;
                    pBundleWrapper.DoSetBundle(pAsync.assetBundle);
                }

                bool bResult = _mapLoadedBundle[strBundleName].pBundle != null;
                if (bResult == false)
                    Debug.LogError($"PreLoadbundle Fail - {strBundleName}");
                OnLoadBundle(strBundleName, bResult);
            }

            public override T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError)
            {
                if(_mapLoadedBundle.ContainsKey(strBundleName) == false)
                {
                    if (bNotLoad_IsError)
                    {
                        Debug.LogError($"Bundle Is Not Loaded! {strBundleName}");
                        return null;
                    }
                    else
                    {
                        string strBundlePath = GetBundlePath(strBundleName);
                        var pBundleNew = AssetBundle.LoadFromFile(strBundlePath);
                        if (pBundleNew == null)
                        {
                            Debug.LogError($"Failed to load AssetBundle! {strBundleName}");
                            return null;
                        }

                        BundleWrapper pBundleWrapper = new BundleWrapper(strBundleName, null);
                        pBundleWrapper.DoSetBundle(pBundleNew);
                        _mapLoadedBundle.Add(strBundleName, pBundleWrapper);
                    }
                }

                var pBundle = _mapLoadedBundle[strBundleName].pBundle;
                T pObject = pBundle.LoadAsset<T>(strPath_With_ExtensionName);
                if (pObject == null)
                {
                    Debug.LogError($"Streaming Asset LoadFail  Bundle : {strBundleName} File Name : {strPath_With_ExtensionName}");
                    return null;
                }

                return pObject;
            }

            private static string GetBundlePath(string strBundleName)
            {
                return Path.Combine(Application.streamingAssetsPath, strBundleName);
            }
        }

        /* public - Field declaration            */

        public delegate void delOnLoadBundle(string strBundleName, bool bIsSuccess);

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

        public void DoPreLoad(string strBundleName, delOnLoadBundle OnLoadBundle)
        {
            StartCoroutine(_pLoadLogic.PreLoadBundle_Coroutine(strBundleName.ToLower(), OnLoadBundle));
        }

        public T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError = true) where T : UnityEngine.Object
        {
            return _pLoadLogic.DoLoad<T>(strBundleName.ToLower(), strPath_With_ExtensionName, bNotLoad_IsError);
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