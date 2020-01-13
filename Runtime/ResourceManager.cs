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

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 
/// </summary>
public class ResourceManager : CSingletonDynamicMonoBase<ResourceManager>
{
    /* const & readonly declaration             */

    const string const_EditorPath = "Assets/00.AssetBundlePath/";

    /* enum & struct declaration                */

    public enum EType
    {
        [RegistSubString(".prefab")]
        Prefab,
        [RegistSubString(".asset")]
        ScriptableObject,
    }

    public abstract class ResourceLoadLogicBase
    {
        abstract public T Load<T>(string strPath, EType eType) where T : UnityEngine.Object;
    }

#if UNITY_EDITOR
    public class ResourceLoadLogic_Editor : ResourceLoadLogicBase
    {
        public override T Load<T>(string strPath, EType eType) 
        {
            return AssetDatabase.LoadAssetAtPath<T>($"{const_EditorPath}/{strPath}{eType.ToStringSub()}");
        }
    }
#endif

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    ResourceLoadLogicBase _pLoadLogic = new ResourceLoadLogic_Editor();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public T DoLoad<T>(string strPath, EType eType)
        where T : UnityEngine.Object
    {
        T pObjectOrigin = _pLoadLogic.Load<T>(strPath, eType);
        if(pObjectOrigin == null)
        {
            Debug.LogError($"LoadFail Path : {const_EditorPath}/{strPath}");
            return null;
        }

        return pObjectOrigin;
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