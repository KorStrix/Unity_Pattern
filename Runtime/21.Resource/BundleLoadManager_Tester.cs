#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-20 오후 3:42:36
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity_Pattern;
using static Unity_Pattern.BundleLoadManager;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceLoadManager_Tester : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [System.Serializable]
        public class LoadType
        {
            public string strFilePath_With_Extension;
            public bool bEnable;
            public string strBundleName;

            public float fDelay;
            public bool bRespawn;
            public Vector3 vecPos;
        }

        /* public - Field declaration            */

        public List<LoadType> listLoadType = new List<LoadType>();
        public BundleLoadManager.EBundleLoadLogic eLoadType;

        /* protected & private - Field declaration         */


        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/


        // ========================================================================== //

        /* protected - Override & Unity API         */

        private void OnEnable()
        {
            BundleLoadManager.instance.DoInit(eLoadType);
            for (int i = 0; i < listLoadType.Count; i++)
            {
                if (listLoadType[i].bEnable)
                    StartCoroutine(COLoadData(listLoadType[i]));
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator COLoadData(LoadType pLoadType)
        {
            yield return new WaitForSeconds(pLoadType.fDelay);

            bool bWait = true;
            bool bIsSuccess = false;
            BundleLoadManager.instance.DoPreLoad_Bundle(pLoadType.strBundleName,
                (strBundleName, bResult) =>
                {
                    bWait = false;
                    bIsSuccess = bResult;
                });

            while (bWait)
            {
                yield return null;
            }

            if (bIsSuccess == false)
                yield break;

            if (pLoadType.bRespawn)
            {
                if (pLoadType.strBundleName == "Prefab")
                {
                    GameObject pObject = BundleLoadManager.instance.DoLoad<GameObject>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
                    Transform pTransformCopy = Instantiate(pObject).transform;
                    pTransformCopy.position = pLoadType.vecPos;

                    Renderer pRenderer = pTransformCopy.GetComponentInChildren<Renderer>();
                    pRenderer.material.shader = Shader.Find(pRenderer.material.shader.name);

                }
                else if (pLoadType.strBundleName == "Sprite")
                {
                    GameObject pObject = new GameObject("SpriteRenderer");
                    SpriteRenderer pSprite = pObject.AddComponent<SpriteRenderer>();
                    pSprite.sprite = BundleLoadManager.instance.DoLoad<Sprite>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
                    pSprite.transform.position = pLoadType.vecPos;
                    pSprite.transform.localScale = Vector3.one * 0.1f;
                }
                else if (pLoadType.strBundleName == "Terrain")
                {
                    GameObject pObject = BundleLoadManager.instance.DoLoad<GameObject>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
                    Transform pTransformCopy = Instantiate(pObject).transform;
                    pTransformCopy.position = pLoadType.vecPos;
                }
                else
                {
                    Debug.LogError("Not Define Test Bundle - " + pLoadType.strBundleName);
                }
            }
            else
            {
                BundleLoadManager.instance.DoLoad<Material>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
            }

            yield return null;
        }

        #endregion Private
    }
}