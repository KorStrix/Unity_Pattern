#region Header
/*	============================================
 *	Author 			    	: Require PlayerPref Key : "Author"
 *	Initial Creation Date 	: 2020-10-14
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
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
    public class InstantiateObject : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [System.Serializable]
        public class InstantiateInfo
        {
            public GameObject pObjectInstantiate;
            public KeyCode eKeycode;
        }

        /* public - Field declaration               */

        public List<InstantiateInfo> listInstantiateInfo = new List<InstantiateInfo>();

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        void Update()
        {
            foreach (var pInfo in listInstantiateInfo)
            {
                if (Input.GetKeyDown(pInfo.eKeycode))
                    Instantiate(pInfo.pObjectInstantiate);
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}
