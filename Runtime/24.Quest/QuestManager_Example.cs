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

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class QuestManager_Example : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public class QuestData_Example : IQuestData
        {
            public string strQuestKey { get; private set; }
            public string strQuestDescription { get; private set; }

            public string GetQuestProgressDescription(EQuestProgress eProgress)
            {
                switch (eProgress)
                {
                    case EQuestProgress.None: return strQuestKey + " ����Ʈ �ȹް�����";
                    case EQuestProgress.In_Progress: return strQuestKey + " ����Ʈ ������";
                    case EQuestProgress.Success: return strQuestKey + " ����Ʈ ����";
                    case EQuestProgress.Fail: return strQuestKey + " ����Ʈ ����";
                    default: return strQuestKey + " ����";
                }
            }

            public QuestData_Example(string strQuestKey, string strQuestDescription)
            {
                this.strQuestKey = strQuestKey; this.strQuestDescription = strQuestDescription;
            }
        }

        public class QuestProgressData_Example : IQuestProgressData
        {
            public string strQuestKey { get; private set; }
            public EQuestProgress eQuestProgress { get; private set; }

            public ObservableCollection<OnUpdateQuestMsg> OnUpdateQuest { get; private set; } = new ObservableCollection<OnUpdateQuestMsg>();

            public QuestProgressData_Example(string strQuestKey)
            {
                this.strQuestKey = strQuestKey;
            }
        }

        /* protected & private - Field declaration  */

        QuestManager _pQuestManager;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}