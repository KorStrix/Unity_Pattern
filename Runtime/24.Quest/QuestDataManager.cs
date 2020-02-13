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

namespace Unity_Pattern
{
    public enum EQuestProgress
    {
        None,

        In_Progress,
        Success,
        Fail,
    }

    public interface IQuestData
    {
        string strQuestKey { get; }
        string strQuestDescription { get; }
        string GetQuestProgressDescription(IQuestProgressData pProgressData);
    }

    public interface IQuestProgressData
    {
        string strQuestKey { get; }
        EQuestProgress eQuestProgress { get; }
        ObservableCollection<IQuestProgressData> OnUpdateQuest { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class QuestDataManager : CSingletonDynamicMonoBase<QuestDataManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public class QuestData
        {
            public ObservableCollection<QuestData> OnUpdateQuest { get; private set; } = new ObservableCollection<QuestData>();


            public IQuestData pQuestData { get; private set; }
            public IQuestProgressData pQuestProgressData { get; private set; }
            public EQuestProgress eQuestProgress { get; private set; }

            public QuestData(IQuestData pQuestData)
            {
                this.pQuestData = pQuestData;
                this.eQuestProgress = EQuestProgress.None;
            }

            public void Event_SetProgress(IQuestProgressData pQuestProgressData)
            {
                this.pQuestProgressData = pQuestProgressData;

                if (pQuestProgressData == null)
                {
                    this.eQuestProgress = EQuestProgress.None;
                }
                else
                {
                    this.eQuestProgress = pQuestProgressData.eQuestProgress;
                    pQuestProgressData.OnUpdateQuest.Subscribe += OnUpdateQuest_Subscribe;
                }
            }

            private void OnUpdateQuest_Subscribe(IQuestProgressData pMessage)
            {
                eQuestProgress = pMessage.eQuestProgress;
                OnUpdateQuest.DoNotify(this);
            }
        }

        /* public - Field declaration               */

        public ObservableCollection<QuestData> OnChange_QuestProgress { get; private set; } = new ObservableCollection<QuestData>();


        public IReadOnlyDictionary<string, QuestData> mapQuestData => _mapQuestData;

        /* protected & private - Field declaration  */

        Dictionary<string, QuestData> _mapQuestData = new Dictionary<string, QuestData>();

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoInit_QuestData<TQuestData>(TQuestData[] arrSourceData, params IQuestProgressData[] arrProgressData)
            where TQuestData : IQuestData
        {
            Dictionary<string, IQuestData> _mapQuestData_Source;
            Dictionary<string, IQuestProgressData> _mapQuestData_Progress;
            _mapQuestData.Clear();

            try
            {
                _mapQuestData_Source = arrSourceData.ToDictionary(p => p.strQuestKey, p => (IQuestData)p);
                _mapQuestData_Progress = arrProgressData.ToDictionary(p => p.strQuestKey);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{nameof(QuestDataManager)}-{nameof(DoInit_QuestData)} - Fail - {e}", this);
                return;
            }

            foreach(var pQuestSource in _mapQuestData_Source.Values)
            {
                string strQuestKey = pQuestSource.strQuestKey;
                QuestData pQuestData = new QuestData(pQuestSource);
                pQuestData.OnUpdateQuest.Subscribe += OnUpdateQuest_Subscribe;

                IQuestProgressData pProgressData;
                if (_mapQuestData_Progress.TryGetValue(strQuestKey, out pProgressData))
                    pQuestData.Event_SetProgress(pProgressData);

                _mapQuestData.Add(strQuestKey, pQuestData);
            }
        }

        public void DoAdd_QuestProgress(IQuestProgressData pProgressData)
        {
            string strQuestKey = pProgressData.strQuestKey;
            if (_mapQuestData.ContainsKey(strQuestKey))
            {
                Debug.LogError("Error");
                return;
            }

            _mapQuestData[strQuestKey].Event_SetProgress(pProgressData);
        }

        public void DoRemove_QuestProgress(string strQuestKey)
        {
            if (_mapQuestData.ContainsKey(strQuestKey) == false)
            {
                Debug.LogError("Error");
                return;
            }

            _mapQuestData[strQuestKey].Event_SetProgress(null);
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

        private void OnUpdateQuest_Subscribe(QuestData pData)
        {
            OnChange_QuestProgress.DoNotify(pData);
        }

        #endregion Private
    }
}