#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-20 오후 2:30:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class InputExtension_Tester_ZoomInOut : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public float FOV_Max = 100f;
    public float FOV_Min = 20f;
    public float fZoomSpeed = 5f;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void Update()
    {
        float fChangedDelta = InputExtension.GetZoomInOut_ChangedDelta();
        if(fChangedDelta != 0f)
        {
            Debug.Log(fChangedDelta);
            Camera.main.fieldOfView += fChangedDelta * fZoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, FOV_Min, FOV_Max);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}