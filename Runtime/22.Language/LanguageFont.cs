#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-11
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
    /// 언어별 폰트파일을 Text에 할당하는 컴포넌트.
    /// </summary>
    [ExecuteInEditMode]
    public class LanguageFont : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        static Font g_pArialFont;

        [Header("없으면 GetComponentInChildren 호출")]
        public Text pText;

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

#if UNITY_EDITOR
        private void Reset()
        {
            if (Application.isPlaying == false)
                UpdateInEditor();
        }

        private void Update()
        {
            if(Application.isPlaying == false)
                UpdateInEditor();
        }

        private void UpdateInEditor()
        {
            LanguageFont[] arrSameComponent = GetComponents<LanguageFont>();
            if(arrSameComponent.Length > 1)
            {
                for (int i = 1; i < arrSameComponent.Length; i++)
                    DestroyImmediate(arrSameComponent[i]);
            }

            if (pText == null)
                pText = GetComponentInChildren<Text>();

            if (g_pArialFont == null)
                g_pArialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            if (pText.font != g_pArialFont)
                pText.font = g_pArialFont;
        }
#endif

        protected override void OnAwake()
        {
            base.OnAwake();

            if (pText == null)
                pText = GetComponentInChildren<Text>();
        }

        protected override void OnEnableObject()
        {
            base.OnEnableObject();

#if UNITY_EDITOR
            if (Application.isEditor && Application.isPlaying == false)
                return;
#endif

            LanguageManager.instance.OnSetFont.Subscribe_And_Listen_CurrentData += OnSetFont;
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            if (bIsQuit_Application)
                return;

#if UNITY_EDITOR
            if (Application.isEditor && Application.isPlaying == false)
                return;
#endif

            LanguageManager.instance.OnSetFont.DoRemove_Observer(OnSetFont);
        }

        private void OnSetFont(Font pFont)
        {
            pText.font = pFont;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}