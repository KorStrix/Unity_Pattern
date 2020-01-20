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
using static Unity_Pattern.ResourceLoadManager;

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
        public string strBundleName;

        public float fDelay;
        public bool bRespawn;
        public Vector3 vecPos;
    }

    public enum ELoadType
    {
        Editor,
        StreamingAsset,
        CDN,
    }

    /* public - Field declaration            */

    public List<LoadType> listLoadType = new List<LoadType>();
    public ELoadType eLoadType;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void OnEnable()
    {
        switch (eLoadType)
        {
#if UNITY_EDITOR
            case ELoadType.Editor: ResourceLoadManager.instance.DoInit(new ResourceLoadLogic_Editor()); break;
#endif

            case ELoadType.StreamingAsset: ResourceLoadManager.instance.DoInit(new ResourceLoadLogic_StreamingAsset()); break;

            case ELoadType.CDN:
                break;
        }

        for(int i = 0; i < listLoadType.Count; i++)
        {
            StartCoroutine(COLoadData(listLoadType[i]));
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator COLoadData(LoadType pLoadType)
    {
        yield return new WaitForSeconds(pLoadType.fDelay);

        if(pLoadType.bRespawn)
        {
            if(pLoadType.strBundleName == "Prefab")
            {
                GameObject pObject = ResourceLoadManager.instance.DoLoad<GameObject>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
                Transform pTransformCopy = Instantiate(pObject).transform;
                pTransformCopy.position = pLoadType.vecPos;
            }
            else if(pLoadType.strBundleName == "Sprite")
            {
                GameObject pObject = new GameObject("SpriteRenderer");
                SpriteRenderer pSprite = pObject.AddComponent<SpriteRenderer>();
                pSprite.sprite = ResourceLoadManager.instance.DoLoad<Sprite>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
                pSprite.transform.position = pLoadType.vecPos;
                pSprite.transform.localScale = Vector3.one * 0.1f;
            }
        }
        else
        {
            ResourceLoadManager.instance.DoLoad<Material>(pLoadType.strBundleName, pLoadType.strFilePath_With_Extension);
        }

        yield return null;
    }

    #endregion Private
}