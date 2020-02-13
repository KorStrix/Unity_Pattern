#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-10
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class LanguageText : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public string strLanguageKey;

        [Header("없으면 GetComponentInChildren 호출")]
        public Text pText;

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            if(pText == null)
                pText = GetComponentInChildren<Text>();
        }

        protected override void OnEnableObject()
        {
            base.OnEnableObject();

            LanguageManager.instance.OnSetLanguage.Subscribe_And_Listen_CurrentData += OnSetLanguage_Subscribe_And_Listen_CurrentData;
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            if (bIsQuit_Application)
                return;

            LanguageManager.instance.OnSetLanguage.DoRemove_Listener(OnSetLanguage_Subscribe_And_Listen_CurrentData);
        }

        private void OnSetLanguage_Subscribe_And_Listen_CurrentData(SystemLanguage pMessage)
        {
            pText.text = LanguageManager.instance.GetText(strLanguageKey);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}