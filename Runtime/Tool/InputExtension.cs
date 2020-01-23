#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-20 오후 2:29:41
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public static class InputExtension
{
    static public float GetZoomInOut_ChangedDelta()
    {
#if UNITY_EDITOR
        return -Input.mouseScrollDelta.y;
#elif UNITY_ANDROID || UNITY_IOS
        if(Input.touchCount >= 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            return prevTouchDeltaMag - touchDeltaMag;
        }
#else
        return 0f;
#endif
    }
}