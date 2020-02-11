#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-08 오후 12:54:55
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>

    [RequireComponent(typeof(Camera))]
    public class CameraManager : CSingletonMonoBase<CameraManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public int iFOV_Max = 60;
        public int iFOV_Min = 30;

        public bool bUpdate_Aspect = true;

        public float fScreenWidth = 16f;
        public float fScreenHeight = 9f;

        /* protected & private - Field declaration         */

        Camera _pCamera = null;

        float _fScreenWidth_Last;
        float _fScreenHeight_Last;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoSetZoomInOut(float fFOV_0_1)
        {
            _pCamera.fieldOfView = Mathf.Lerp(iFOV_Min, iFOV_Max, fFOV_0_1);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pCamera = GetComponent<Camera>();
        }

        // http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html
        private void Update()
        {
            if(bUpdate_Aspect)
            {
                float fScreenWidth_Current = (float)Screen.width;
                float fScreenHeight_Current = (float)Screen.height;
                if (fScreenWidth_Current == _fScreenWidth_Last && fScreenHeight_Current == _fScreenHeight_Last)
                    return;

                _fScreenWidth_Last = fScreenWidth_Current;
                _fScreenHeight_Last = fScreenHeight_Current;

                // set the desired aspect ratio (the values in this example are
                // hard-coded for 16:9, but you could make them into public
                // variables instead so you can set them at design time)
                float fTargetAspect = fScreenWidth / fScreenHeight;

                // determine the game window's current aspect ratio
                float windowaspect = fScreenWidth_Current / fScreenHeight_Current;

                // current viewport height should be scaled by this amount
                float scaleheight = windowaspect / fTargetAspect;

                // obtain camera component so we can modify its viewport

                // if scaled height is less than current height, add letterbox
                if (scaleheight < 1.0f)
                {
                    Rect rect = _pCamera.rect;

                    rect.width = 1.0f;
                    rect.height = scaleheight;
                    rect.x = 0;
                    rect.y = (1.0f - scaleheight) / 2.0f;

                    _pCamera.rect = rect;
                }
                else // add pillarbox
                {
                    float scalewidth = 1.0f / scaleheight;

                    Rect rect = _pCamera.rect;

                    rect.width = scalewidth;
                    rect.height = 1.0f;
                    rect.x = (1.0f - scalewidth) / 2.0f;
                    rect.y = 0;

                    _pCamera.rect = rect;
                }
            }
        }

        /* protected - [abstract & virtual]         */

        // ========================================================================== //

        #region Private

        #endregion Private
    }
}