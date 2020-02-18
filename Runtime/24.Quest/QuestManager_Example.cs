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
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class QuestManager_Example : CSingletonDynamicMonoBase<QuestManager_Example>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EQuestKey_Example
        {
            Kill_Orc,
            Kill_Goblin,

            Get_Stone,
            Get_Wood,
        }

        public enum EQuestMonsterKey_Example
        {
            Orc,
            Goblin,
        }

        public enum EQuestItemKey_Example
        {
            Stone,
            Wood,
        }
        

        [System.Serializable]
        public class QuestDataExample : IAchievementData
        {
            public string strAchievementKey => eQuestKey.ToString();
            string IAchievementData.strAchievementDescription => strQuestDescription;
            int IAchievementData.iAchievementCount => iAchievementCount;

            public EQuestKey_Example eQuestKey;
            public string strQuestDescription;
            public int iAchievementCount;

            public string GetAchievementProgressDescription(IAchievementProgressData pProgressData, EAchieveProgress eQuestProgress)
            {
                switch (eQuestProgress)
                {
                    case EAchieveProgress.None: return strAchievementKey + " 퀘스트 안받고있음";
                    case EAchieveProgress.In_Progress: return strAchievementKey + " 퀘스트 진행중";
                    case EAchieveProgress.Success: return strAchievementKey + " 퀘스트 성공";
                    case EAchieveProgress.Fail: return strAchievementKey + " 퀘스트 실패";
                    case EAchieveProgress.GiveUp: return strAchievementKey + " 퀘스트 포기";

                    default: return strAchievementKey + " 에러";
                }
            }

            public QuestDataExample(EQuestKey_Example eQuestKey, string strQuestDescription, int iAchievementCount)
            {
                this.eQuestKey = eQuestKey; this.strQuestDescription = strQuestDescription; this.iAchievementCount = iAchievementCount;
            }
        }

        [System.Serializable]
        public class QuestProgressData_Example : IAchievementProgressData
        {
            public ObservableCollection<IAchievementProgressData> OnUpdateAchievemenet { get; private set; } = new ObservableCollection<IAchievementProgressData>();
            public string strAchievementKey => eQuestKey.ToString();
            public EAchieveProgress eAchieveProgress
            {
                get => eQuestProgress;
                set => eQuestProgress = value;
            }

            int IAchievementProgressData.iAchievementCount => iAchievementCount;

            public EQuestKey_Example eQuestKey;
            public EAchieveProgress eQuestProgress;
            public int iAchievementCount;

            public QuestProgressData_Example(EQuestKey_Example eQuestKey, int iAchievementCount)
            {
                this.eQuestKey = eQuestKey; this.iAchievementCount = iAchievementCount;
            }
        }

        /* public - Field declaration               */

        public AchievementDataManager pQuestDataManager { get; private set; }

        public EQuestMonsterKey_Example eMonsterKey;
        public EQuestItemKey_Example eItemKey;

        public List<QuestDataExample> listQuestData = new List<QuestDataExample>();
        public List<QuestProgressData_Example> listQuestProgressData = new List<QuestProgressData_Example>();

        /* protected & private - Field declaration  */

        Dictionary<EQuestMonsterKey_Example, QuestProgressData_Example> _mapQuestProgress_Monster = new Dictionary<EQuestMonsterKey_Example, QuestProgressData_Example>();
        Dictionary<EQuestItemKey_Example, QuestProgressData_Example> _mapQuestProgress_Item = new Dictionary<EQuestItemKey_Example, QuestProgressData_Example>();
        Dictionary<EQuestKey_Example, QuestDataExample> _mapQuestData = new Dictionary<EQuestKey_Example, QuestDataExample>();

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoResetProgress_Quest()
        {
            _mapQuestProgress_Monster.Clear();
            _mapQuestProgress_Item.Clear();
        }

        public void DoKillMonster(EQuestMonsterKey_Example eMonsterKey)
        {
            QuestProgressData_Example pProgressData;
            if (_mapQuestProgress_Monster.TryGetValue(eMonsterKey, out pProgressData) == false)
            {
                Debug.Log($"{eMonsterKey}를 죽였다. 근데 관련 퀘스트가 없다..");
                return;
            }

            ++pProgressData.iAchievementCount;
            Debug.Log($"{eMonsterKey}를 죽였다. 관련 퀘스트 업데이트중");
            pProgressData.OnUpdateAchievemenet.DoNotify(pProgressData);
        }

        public void DoGetItem(EQuestItemKey_Example eItemKey)
        {
            QuestProgressData_Example pProgressData;
            if (_mapQuestProgress_Item.TryGetValue(eItemKey, out pProgressData) == false)
            {
                Debug.Log($"{eItemKey}를 얻었다. 근데 관련 퀘스트가 없다..");
                return;
            }

            ++pProgressData.iAchievementCount;
            Debug.Log($"{eItemKey}를 얻었다. 관련 퀘스트 업데이트중");
            pProgressData.OnUpdateAchievemenet.DoNotify(pProgressData);
        }

        public void DoGiveUpQuest(EQuestKey_Example eQuestKey)
        {
            QuestDataExample pQuestData;
            if (_mapQuestData.TryGetValue(eQuestKey, out pQuestData) == false)
            {
                Debug.Log($"{eQuestKey} 퀘스트를 포기하려 하는데 퀘스트 데이터를 못찾았다..");
                return;
            }

            Debug.Log($"{eQuestKey} 퀘스트를 포기했다");
            pQuestDataManager.DoSetForce_AchievementProgress(eQuestKey.ToString(), EAchieveProgress.GiveUp);
            // pQuestDataManager.DoRemove_AchievementProgress(eQuestKey.ToString());
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            pQuestDataManager = GetComponent<AchievementDataManager>();
            pQuestDataManager.OnChange_AchievementProgress.Subscribe += OnChange_QuestProgress;

            _mapQuestProgress_Monster.Clear();
            _mapQuestProgress_Item.Clear();
            pQuestDataManager.DoInit(listQuestData.ToArray(), new IAchievementLogic[] { new AchievementLogic_CountOver() }, listQuestProgressData.ToArray());
            _mapQuestData = listQuestData.ToDictionary(p => p.eQuestKey);

            for (int i = 0; i < listQuestProgressData.Count; i++)
            {
                QuestProgressData_Example pData = listQuestProgressData[i];
                EQuestKey_Example eQuestKey = pData.eQuestKey;

                QuestDataExample pQuestData;
                if(_mapQuestData.TryGetValue(eQuestKey, out pQuestData) == false)
                {
                    Debug.LogError("Error");
                    continue;
                }

                switch (eQuestKey)
                {
                    case EQuestKey_Example.Kill_Orc: _mapQuestProgress_Monster.Add(EQuestMonsterKey_Example.Orc, pData); break;
                    case EQuestKey_Example.Kill_Goblin: _mapQuestProgress_Monster.Add(EQuestMonsterKey_Example.Goblin, pData); break;
                    case EQuestKey_Example.Get_Stone: _mapQuestProgress_Item.Add(EQuestItemKey_Example.Stone, pData); break;
                    case EQuestKey_Example.Get_Wood: _mapQuestProgress_Item.Add(EQuestItemKey_Example.Wood, pData); break;

                    default:
                        break;
                }
            }
        }

        private void OnChange_QuestProgress(AchievementDataManager.AchievementData pMessage)
        {
            Debug.Log($"{pMessage.strAchievementKey} - {pMessage.GetProgressDescription()}");
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(QuestManager_Example))]
    public class QuestManager_Example_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            QuestManager_Example pTarget = target as QuestManager_Example;

            if (GUILayout.Button($"{nameof(pTarget.DoAwake_Force)}"))
            {
                pTarget.DoAwake_Force();
                Debug.Log($"{nameof(pTarget.DoAwake_Force)}");
            }

            if (GUILayout.Button($"{nameof(pTarget.DoResetProgress_Quest)}"))
            {
                pTarget.DoResetProgress_Quest();
                Debug.Log($"{nameof(pTarget.DoResetProgress_Quest)}");
            }

            GUILayout.Space(10);

            if (GUILayout.Button($"{nameof(pTarget.DoKillMonster)}"))
            {
                pTarget.DoKillMonster(pTarget.eMonsterKey);
                Debug.Log($"{nameof(pTarget.DoKillMonster)}");
            }

            if (GUILayout.Button($"{nameof(pTarget.DoGetItem)}"))
            {
                pTarget.DoGetItem(pTarget.eItemKey);
                Debug.Log($"{nameof(pTarget.DoGetItem)}");
            }
        }
    }

#endif
}