#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-01-23
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
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
    public class EffectPlayer : CObjectBase, IEffectPlayer
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public ObservableCollection<EffectPlayArg> OnFinish_Effect => throw new System.NotImplementedException();
        public string strEffectName => throw new System.NotImplementedException();

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        public void IEffectPlayer_PlayEffect()
        {
        }

        public void IEffectPlayer_PlayEffect_Loop()
        {
        }

        public void IEffectPlayer_StopEffect(bool bNotify_OnFinishEffect)
        {
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}