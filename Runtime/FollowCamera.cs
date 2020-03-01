#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-12 오전 10:10:44
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity_Pattern;

/// <summary>
/// 
/// </summary>

[ExecuteInEditMode]
public class FollowCamera : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Flags]
    public enum ERotateLockOption
    { 
        None = 0,

        X = 1 << 0,
        Y = 1 << 1,

        All,
    }


    /* public - Field declaration            */

    public bool bUseSimple;
    public Camera pCamera;
    public ERotateLockOption eLockOption;
    public bool bFlipX = true;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    private void Update()
    {
        if (pCamera == null)
        {
            pCamera = Camera.main;
        }

        Vector3 vecCameraPosition = pCamera.transform.position;

        if (bUseSimple)
        {
            Vector3 vecGap = vecCameraPosition - transform.position;

            transform.rotation = Quaternion.identity;
            if (vecGap.z > 0)
                transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
        else
        {
            if (eLockOption == ERotateLockOption.None)
            {
                if (bFlipX)
                    transform.LookAt(-vecCameraPosition);
                else
                    transform.LookAt(vecCameraPosition);
            }
            else
            {
                Vector3 vecPosition = vecCameraPosition;

                if (bFlipX)
                    vecPosition *= -1;

                if ((eLockOption & ERotateLockOption.Y) == ERotateLockOption.Y)
                    vecPosition.x = transform.position.x;

                if ((eLockOption & ERotateLockOption.X) == ERotateLockOption.X)
                    vecPosition.y = transform.position.y;

                transform.LookAt(vecPosition);
            }
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}