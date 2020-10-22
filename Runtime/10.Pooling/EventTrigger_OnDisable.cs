#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-11 오후 12:54:37
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 유니티 이벤트 - Disable, Destroy 이벤트 트리거
/// </summary>
public class EventTrigger_OnDisable : MonoBehaviour
{
    public event System.Action<GameObject> OnDisableObject;
    public event System.Action<GameObject> OnDestroyObject;

    private void OnDisable()
    {
        Invoke(nameof(ExecuteOnDisable), 0.01f);
    }

    private void ExecuteOnDisable()
    {
        if (OnDisableObject != null)
        {
            OnDisableObject(gameObject);
            OnDisableObject = null;
        }
    }

    private void OnDestroy()
    {
        if (OnDestroyObject != null)
        {
            OnDestroyObject(gameObject);
            OnDestroyObject = null;
        }
    }
}
