#region Header
/*	============================================
 *	Author 			        : Strix
 *	Initial Creation Date 	: 2020-02-11
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity_Pattern
{
    public enum EAchieveProgress
    {
        None,

        In_Progress,

        Success,
        Fail,

        GiveUp,
    }

    public interface IAchievementData
    {
        string strAchievementKey { get; }
        string strAchievementDescription { get; }
        int iAchievementCount { get; }
        string GetAchievementProgressDescription(IAchievementProgressData pProgressData, EAchieveProgress eProgress);
    }

    public interface IAchievementProgressData
    {
        string strAchievementKey { get; }
        EAchieveProgress eAchieveProgress { get; set; }
        int iAchievementCount { get; }
        ObservableCollection<IAchievementProgressData> OnUpdateAchievemenet { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AchievementDataManager : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public class AchievementData
        {
            public ObservableCollection<AchievementData> OnUpdateAchieve { get; private set; } = new ObservableCollection<AchievementData>();
            public IEnumerable<IAchievementLogic> OnCalculateAchieve { get; private set; }


            public string strAchievementKey { get; private set; }
            public IAchievementData pAchievementData { get; private set; }
            public IAchievementProgressData pProgressData { get; private set; }

            public EAchieveProgress eProgress => pProgressData == null ? EAchieveProgress.None : pProgressData.eAchieveProgress;

            public AchievementData(IAchievementData pAchievemenetData, IEnumerable<IAchievementLogic> OnCalculateAchieve)
            {
                this.strAchievementKey = pAchievemenetData.strAchievementKey;
                this.pAchievementData = pAchievemenetData;
                this.OnCalculateAchieve = OnCalculateAchieve;
            }

            public void Event_SetProgressData(IAchievementProgressData pProgressData)
            {
                this.pProgressData = pProgressData;

                if(pProgressData != null)
                {
                    OnUpdateAchievement_Subscribe(pProgressData);
                    pProgressData.OnUpdateAchievemenet.Subscribe += OnUpdateAchievement_Subscribe;
                }
            }

            public string GetProgressDescription()
            {
                return pAchievementData.GetAchievementProgressDescription(pProgressData, eProgress);
            }

            private void OnUpdateAchievement_Subscribe(IAchievementProgressData pProgressData)
            {
                foreach (var pLogic in OnCalculateAchieve)
                    pProgressData.eAchieveProgress = pLogic.Calculate_AchievementProgress(this, pProgressData.eAchieveProgress);

                OnUpdateAchieve.DoNotify(this);
            }
        }

        /* public - Field declaration               */

        public ObservableCollection<AchievementData> OnChange_AchievementProgress { get; private set; } = new ObservableCollection<AchievementData>();


        public IReadOnlyDictionary<string, AchievementData> mapAchievementData => _mapAchievementData;

        /* protected & private - Field declaration  */

        Dictionary<string, AchievementData> _mapAchievementData = new Dictionary<string, AchievementData>();
        List<IAchievementLogic> _listAchievementLogicList;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInit<TAchievementData>(TAchievementData[] arrSourceData, IAchievementLogic[] arrAchievementLogicList, params IAchievementProgressData[] arrProgressData)
            where TAchievementData : IAchievementData
        {
            Dictionary<string, IAchievementData> mapAchivementData_Source;
            Dictionary<string, IAchievementProgressData> mapAchievementData_Progress;
            _mapAchievementData.Clear();
            _listAchievementLogicList = arrAchievementLogicList.OrderBy(p => p.iAchievementLogic_Order).ToList();

            try
            {
                mapAchivementData_Source = arrSourceData.ToDictionary(p => p.strAchievementKey, p => (IAchievementData)p);
                mapAchievementData_Progress = arrProgressData.ToDictionary(p => p.strAchievementKey);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{nameof(AchievementDataManager)}-{nameof(DoInit)} - Fail - {e}", this);
                return;
            }

            foreach(var pQuestSource in mapAchivementData_Source.Values)
            {
                string strQuestKey = pQuestSource.strAchievementKey;
                AchievementData pQuestData = new AchievementData(pQuestSource, _listAchievementLogicList);
                pQuestData.OnUpdateAchieve.Subscribe += OnUpdateAchievement_Subscribe;

                IAchievementProgressData pProgressData;
                if (mapAchievementData_Progress.TryGetValue(strQuestKey, out pProgressData))
                    pQuestData.Event_SetProgressData(pProgressData);

                _mapAchievementData.Add(strQuestKey, pQuestData);
            }
        }

        public AchievementData DoGet_AchievementData(string strAchievementKey)
        {
            AchievementData pData;
            _mapAchievementData.TryGetValue(strAchievementKey, out pData);

            return pData;
        }

        public void DoAdd_AchievementProgress(IAchievementProgressData pProgressData)
        {
            string strKey = pProgressData.strAchievementKey;
            if (_mapAchievementData.ContainsKey(strKey))
            {
                Debug.LogError($"{name} - {nameof(DoAdd_AchievementProgress)} - Error Already ContainKey({strKey})");
                return;
            }

            _mapAchievementData[strKey].Event_SetProgressData(pProgressData);
        }

        public void DoSetForce_AchievementProgress(string strAchievementKey, EAchieveProgress eProgress, bool bIsNotifyObserver = true)
        {
            AchievementData pData = DoGet_AchievementData(strAchievementKey);
            pData.pProgressData.eAchieveProgress = eProgress;

            if(bIsNotifyObserver)
                pData.OnUpdateAchieve.DoNotify(pData);
        }

        public void DoRemove_AchievementProgress(string strAchievementKey)
        {
            if (_mapAchievementData.ContainsKey(strAchievementKey) == false)
            {
                Debug.LogError($"{name} - {nameof(DoRemove_AchievementProgress)} - Error Not Found Key {strAchievementKey}");
                return;
            }

            _mapAchievementData[strAchievementKey].Event_SetProgressData(null);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(gameObject);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void OnUpdateAchievement_Subscribe(AchievementData pData)
        {
            OnChange_AchievementProgress.DoNotify(pData);
        }

        #endregion Private
    }
}