#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-06
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
 *  
 *  참고한 링크
 *  https://www.youtube.com/watch?v=TYNF5PifSmA
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
    public class CameraScreenResolution : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public Camera pCamera;

        public bool maintainWidth = true;
        [Range(-1, 1)]
        public int adaptPosition;

        /* protected & private - Field declaration  */


        float defaultWidth;
        float defaultHeight;

        Vector3 CameraPos;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            if (pCamera == null)
                DoInit(Camera.main);
            else
                DoInit(pCamera);
        }

        private void DoInit(Camera pCamera)
        {
            this.pCamera = pCamera;

            CameraPos = pCamera.transform.position;
            defaultHeight = pCamera.orthographicSize;
            defaultWidth = pCamera.orthographicSize * pCamera.aspect;
        }

        private void Update()
        {
            if(maintainWidth)
            {
                pCamera.orthographicSize = defaultWidth / pCamera.aspect;
                pCamera.transform.position = new Vector3(CameraPos.x, adaptPosition * (defaultHeight - pCamera.orthographicSize), CameraPos.z);
            }
            else
            {
                pCamera.transform.position = new Vector3(adaptPosition * (defaultWidth - pCamera.orthographicSize * pCamera.aspect), CameraPos.y, CameraPos.z);
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}