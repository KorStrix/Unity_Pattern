#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-13 오후 8:20:54
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 유니티 물리 이벤트(TriggerEnter 등) 발생시 알리미.
/// <para>2D전용</para>
/// </summary>
public class PhysicsMessageSender2D : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EColliderShape
    {
        Error,

        Circle,
        Box,
    }

    public enum EPhysicsEvent
    {
        Enter, Stay, Exit
    }

    /* public - Field declaration            */

    public delegate void del_OnTriggerEvent(Rigidbody2D pRigidbody_Sender, Collider2D pCollider_Sender, EColliderShape eColliderShape, EPhysicsEvent ePhysicsEvent, Collider2D collision_Target);
    public event del_OnTriggerEvent OnTriggerEvent;
    public event del_OnTriggerEvent OnTriggerEvent_Stay;

    public delegate void del_OnCollisionEvent(Rigidbody2D pRigidbody_Sender, Collider2D pCollider_Sender, EColliderShape eColliderShape, EPhysicsEvent ePhysicsEvent, Collision2D collision_Target);
    public event del_OnCollisionEvent OnCollisionEvent;
    public event del_OnCollisionEvent OnCollisionEvent_Stay;

    /* protected & private - Field declaration         */

    Rigidbody2D _pRigidbody;
    Collider2D _pCollider;

    EColliderShape _eColliderShape;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoClear_AllListener_OnTriggerEvent()
    {
        OnTriggerEvent = null;
        OnTriggerEvent_Stay = null;
    }

    public void DoClear_AllListener_OnCollisionStay()
    {
        OnCollisionEvent = null;
        OnCollisionEvent_Stay = null;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void Awake()
    {
        _pRigidbody = GetComponent<Rigidbody2D>();
        _pCollider = GetComponent<Collider2D>();

        _eColliderShape = EColliderShape.Error;
        if (_pCollider is CircleCollider2D)
            _eColliderShape = EColliderShape.Circle;
        else if (_pCollider is BoxCollider2D)
            _eColliderShape = EColliderShape.Box;

        if (_eColliderShape == EColliderShape.Error)
        {
            Debug.LogError(name + " 컬라이더 타입을 찾지 못했습니다", this);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEvent?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Enter, collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEvent_Stay?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Stay, collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerEvent?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Exit, collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEvent?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Enter, collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEvent_Stay?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Stay, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionEvent?.Invoke(_pRigidbody, _pCollider, _eColliderShape, EPhysicsEvent.Exit, collision);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}