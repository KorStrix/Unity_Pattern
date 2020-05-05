#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-13 오후 9:55:18
 *	개요 : 
   ============================================ */
#endregion Header

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.U2D;

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

        public enum EBundleLoadLogic
        {
            Editor,
            StreamingAssets,
        }

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
            protected BundleLoadManager _pOwner { get; private set; }
            protected Dictionary<string, BundleWrapper> _mapLoadedBundle { get; private set; }
            public ResourceLoadLogicBase(BundleLoadManager pOwner, Dictionary<string, BundleWrapper> mapLoadedBundle)
            {
                _pOwner = pOwner;
                _mapLoadedBundle = mapLoadedBundle;
            }

            public abstract IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle);

            public abstract T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError) where T : UnityEngine.Object;
        };

        public class ResourceLoadLogic_Editor : ResourceLoadLogicBase
        {
            public ResourceLoadLogic_Editor(BundleLoadManager pOwner, Dictionary<string, BundleWrapper> mapLoadedBundle) : base(pOwner, mapLoadedBundle)
            {
            }

            public override IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle)
            {
                OnLoadBundle(strBundleName, true);
                yield break;
            }

            public override T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError)
            {
#if UNITY_EDITOR
                string strTotalPath = $"{const_EditorPath}/{strBundleName}/{strPath_With_ExtensionName}";

                T pObject = default(T);
                
                try
                {
                    // Debug.Log($"{_pOwner.name} {strTotalPath}");
                    pObject = AssetDatabase.LoadAssetAtPath<T>(strTotalPath);
                }
                catch (Exception e)
                {
                    if(bNotLoad_IsError)
                        Debug.LogError($"{_pOwner.name} LoadFail Path : {strTotalPath}" + e, _pOwner);
                }

                if (bNotLoad_IsError && pObject == null)
                    Debug.LogError($"{_pOwner.name} LoadFail Path : {strTotalPath}", _pOwner);

                return pObject;
#else
                return null;
#endif
            }
        }

        public class ResourceLoadLogic_StreamingAsset : ResourceLoadLogicBase
        {
            public ResourceLoadLogic_StreamingAsset(BundleLoadManager pOwner, Dictionary<string, BundleWrapper> mapLoadedBundle) : base(pOwner, mapLoadedBundle)
            {
            }

            public override IEnumerator PreLoadBundle_Coroutine(string strBundleName, delOnLoadBundle OnLoadBundle)
            {
                bool bLoaded = false;
                if (_mapLoadedBundle.ContainsKey(strBundleName))
                {
                    var pAsyncExist = _mapLoadedBundle[strBundleName].pAsyncOperation;
                    if (pAsyncExist.isDone)
                    {
                        OnLoadBundle(strBundleName, true);
                        yield break;
                    }
                    else
                    {
                        // yield return pAsyncExist; 다른 코루틴에서 같은 yield 탄다고 에러 뱉음
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
                    Debug.LogError($"{_pOwner.name} PreLoadBundle Fail - {strBundleName}", _pOwner);
                OnLoadBundle(strBundleName, bResult);
            }

            public override T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError)
            {
                if(_mapLoadedBundle.ContainsKey(strBundleName) == false)
                {
                    if (bNotLoad_IsError)
                    {
                        Debug.LogError($"{_pOwner.name} Bundle Is Not Loaded! {strBundleName}", _pOwner);
                        return null;
                    }
                    else
                    {
                        string strBundlePath = GetBundlePath(strBundleName);
                        var pBundleNew = AssetBundle.LoadFromFile(strBundlePath);
                        if (pBundleNew == null)
                        {
                            Debug.LogError($"{_pOwner.name} Failed to load AssetBundle! {strBundleName}", _pOwner);
                            return null;
                        }

                        BundleWrapper pBundleWrapper = new BundleWrapper(strBundleName, null);
                        pBundleWrapper.DoSetBundle(pBundleNew);
                        _mapLoadedBundle.Add(strBundleName, pBundleWrapper);
                    }
                }

                if (strPath_With_ExtensionName.Contains("/"))
                {
                    int iCutIndex = strPath_With_ExtensionName.LastIndexOf("/");
                    strPath_With_ExtensionName = strPath_With_ExtensionName.Substring(iCutIndex + 1, strPath_With_ExtensionName.Length - iCutIndex - 1);
                }


                var pBundle = _mapLoadedBundle[strBundleName].pBundle;
                T pObject = pBundle.LoadAsset<T>(strPath_With_ExtensionName);
                if (pObject == null)
                {
                    if(bNotLoad_IsError)
                        Debug.LogError($"{_pOwner.name} Streaming Asset LoadFail  Bundle : {strBundleName} File Name : {strPath_With_ExtensionName}", _pOwner);

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

        Dictionary<string, BundleWrapper> _mapLoadedBundle = new Dictionary<string, BundleWrapper>();

        ResourceLoadLogicBase _pLoadLogic = null;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit(EBundleLoadLogic eLogicName)
        {
            switch (eLogicName)
            {
                case EBundleLoadLogic.Editor: _pLoadLogic = new ResourceLoadLogic_Editor(this, _mapLoadedBundle); break;
                case EBundleLoadLogic.StreamingAssets: _pLoadLogic = new ResourceLoadLogic_StreamingAsset(this, _mapLoadedBundle); break;
                default:
                    break;
            }

        }

        public Coroutine DoPreLoad_Bundle(string strBundleName, delOnLoadBundle OnLoadBundle)
        {
            return StartCoroutine(_pLoadLogic.PreLoadBundle_Coroutine(strBundleName.ToLower(), OnLoadBundle));
        }

        public T DoLoad<T>(string strBundleName, string strPath_With_ExtensionName, bool bNotLoad_IsError = true) where T : UnityEngine.Object
        {
            return _pLoadLogic.DoLoad<T>(strBundleName.ToLower(), strPath_With_ExtensionName, bNotLoad_IsError);
        }

        public COMPONENT DoLoadPrefab<COMPONENT>(string strBundleName, string strPath, bool bNotLoad_IsError = true) where COMPONENT : UnityEngine.Component
        {
            GameObject pObject = _pLoadLogic.DoLoad<GameObject>(strBundleName.ToLower(), strPath + ".prefab", bNotLoad_IsError);
            COMPONENT pComponent = pObject != null ? pObject.GetComponent<COMPONENT>() : null;

            if(bNotLoad_IsError && pComponent == null)
                Debug.LogError($"{strBundleName}/{strPath} - {nameof(DoLoadPrefab)} Fail", this);
            
            return pComponent;
        }

        public Sprite DoLoadSprite_InAtlas(string strBundleName, string strAtlasFileName, string strImageFileName, bool bNotLoad_IsError = true)
        {
            SpriteAtlas pSpriteAtlas = _pLoadLogic.DoLoad<SpriteAtlas>(strBundleName.ToLower(), strAtlasFileName + ".spriteatlas", bNotLoad_IsError);
            if (pSpriteAtlas == null)
            {
                Debug.LogError($"{name} pSpriteAtlas == null // BundleName: {strBundleName} Atlas: {strAtlasFileName} File: {strImageFileName}"); 
                return null;
            }

            Sprite pSprite = pSpriteAtlas.GetSprite(strImageFileName);
            if (pSprite == null)
            {
                Debug.LogError($"{name} pSprite == null // BundleName: {strBundleName} Atlas: {strAtlasFileName} File: {strImageFileName}");
            }

            return pSprite;
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