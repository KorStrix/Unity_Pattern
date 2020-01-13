#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-06 오후 4:59:56
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
abstract public class ObjectBase : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool _bIsExecute_Awake { get; protected set; }

    /* protected & private - Field declaration         */

    Coroutine _pCoroutine_OnAwake;
    Coroutine _pCoroutine_OnEnable;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAwake()
    {
        Awake();
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected void Awake() 
    {
        if (_bIsExecute_Awake)
            return;
        _bIsExecute_Awake = true;

        OnAwake();
    }
    protected void OnEnable()
    {
        OnEnableObject();
    }

    private void OnDisable()
    {
        OnDisableObject();
    }

    /* protected - [abstract & virtual]         */

    virtual protected void OnAwake() 
    {
        if (_pCoroutine_OnAwake != null)
            StopCoroutine(_pCoroutine_OnAwake);
        _pCoroutine_OnAwake = StartCoroutine(OnAwakeCoroutine());
    }

    virtual protected void OnEnableObject() 
    {
        if (_pCoroutine_OnEnable != null)
            StopCoroutine(_pCoroutine_OnEnable);
        _pCoroutine_OnEnable = StartCoroutine(OnEnableCoroutine());
    }

    virtual protected void OnDisableObject() { }

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected IEnumerator OnEnableCoroutine() { yield break; }


    // ========================================================================== //

    #region Private

    #endregion Private
}