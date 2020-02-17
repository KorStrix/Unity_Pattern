#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-16 오후 1:48:42
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
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailRendererWrapper : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */


        /* protected & private - Field declaration         */

        TrailRenderer _pTrailRendrer;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/


        // ========================================================================== //

        /* protected - Override & Unity API         */

        private void Awake()
        {
            _pTrailRendrer = GetComponent<TrailRenderer>();
        }

        private void OnDisable()
        {
            _pTrailRendrer.Clear();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}