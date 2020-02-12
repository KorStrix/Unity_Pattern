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

        public enum EQuestKey_Example
        {
            오크_죽이기,
            고블린_죽이기,

            돌_얻기,
            나무_얻기,
        }

        public enum EQuestMonsterKey_Example
        {
            오크,
            고블린,
        }

        public enum EQuestItemKey_Example
        {
            돌,
            나무,
        }
        

        [System.Serializable]
        public class QuestDataExample_GetItem : IQuestData
        {
            public string strQuestKey => eQuestKey.ToString();
            string IQuestData.strQuestDescription => strQuestDescription;

            public EQuestKey_Example eQuestKey;
            public string strQuestDescription;

            public int iArchievementCount;

            public string GetQuestProgressDescription(IQuestProgressData pProgressData)
            {
                switch (pProgressData.eQuestProgress)
                {
                    case EQuestProgress.None: return strQuestKey + " 퀘스트 안받고있음";
                    case EQuestProgress.In_Progress: return strQuestKey + " 퀘스트 진행중";
                    case EQuestProgress.Success: return strQuestKey + " 퀘스트 성공";
                    case EQuestProgress.Fail: return strQuestKey + " 퀘스트 실패";
                    default: return strQuestKey + " 에러";
                }
            }

            public QuestDataExample_GetItem(EQuestKey_Example eQuestKey, string strQuestDescription)
            {
                this.eQuestKey = eQuestKey; this.strQuestDescription = strQuestDescription;
            }
        }

        [System.Serializable]
        public class QuestProgressData_Example : IQuestProgressData
        {
            public string strQuestKey => eQuestKey.ToString();

            public EQuestKey_Example eQuestKey;
            public EQuestProgress eQuestProgress;

            public int iArchievementCount;

            public ObservableCollection<IQuestProgressData> OnUpdateQuest { get; private set; } = new ObservableCollection<IQuestProgressData>();

            EQuestProgress IQuestProgressData.eQuestProgress => eQuestProgress;
        }

        
        /* public - Field declaration               */

        public List<QuestDataExample_GetItem> listQuestData = new List<QuestDataExample_GetItem>();
        public List<QuestProgressData_Example> listQuestProgressData = new List<QuestProgressData_Example>();

        /* protected & private - Field declaration  */

        Dictionary<EQuestMonsterKey_Example, QuestProgressData_Example> _mapQuestProgress_Monster = new Dictionary<EQuestMonsterKey_Example, QuestProgressData_Example>();
        Dictionary<EQuestItemKey_Example, QuestProgressData_Example> _mapQuestProgress_Item = new Dictionary<EQuestItemKey_Example, QuestProgressData_Example>();
        QuestManager _pQuestManager;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoResetProgress_Quest()
        {

        }

        public void DoKillMonster(EQuestMonsterKey_Example eMonsterKey)
        {
            QuestProgressData_Example pProgressData;
            if(_mapQuestProgress_Monster.TryGetValue(eMonsterKey, out pProgressData) == false)
            {
                Debug.Log($"{eMonsterKey}를 죽였다. 근데 관련 퀘스트가 없다..");
                return;
            }

            Debug.Log($"{eMonsterKey}를 죽였다. 관련 퀘스트 업데이트중");
            pProgressData.OnUpdateQuest.DoNotify(pProgressData);
        }

        public void DoGetItem(EQuestItemKey_Example eItemKey)
        {
            QuestProgressData_Example pProgressData;
            if (_mapQuestProgress_Item.TryGetValue(eItemKey, out pProgressData) == false)
            {
                Debug.Log($"{eItemKey}를 얻었다. 근데 관련 퀘스트가 없다..");
                return;
            }

            Debug.Log($"{eItemKey}를 얻었다. 관련 퀘스트 업데이트중");
            pProgressData.OnUpdateQuest.DoNotify(pProgressData);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            _pQuestManager = GetComponent<QuestManager>();
        }

        private void OnEnable()
        {
            _pQuestManager.DoInit_QuestData(listQuestData.ToArray(), listQuestProgressData.ToArray());

            for(int i = 0; i < listQuestProgressData.Count; i++)
            {
                QuestProgressData_Example pData = listQuestProgressData[i];
                EQuestKey_Example eQuestKey = pData.eQuestKey;

                switch (eQuestKey)
                {
                    case EQuestKey_Example.오크_죽이기: _mapQuestProgress_Monster.Add(EQuestMonsterKey_Example.오크, pData); break;
                    case EQuestKey_Example.고블린_죽이기: _mapQuestProgress_Monster.Add(EQuestMonsterKey_Example.고블린, pData); break;
                    case EQuestKey_Example.돌_얻기: _mapQuestProgress_Item.Add(EQuestItemKey_Example.돌, pData); break;
                    case EQuestKey_Example.나무_얻기: _mapQuestProgress_Item.Add(EQuestItemKey_Example.나무, pData); break;

                    default:
                        break;
                }
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}