#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-12
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    public enum ECombinationResult
    {
        None,

        Fail,
        Success,
    }

    public interface ICombinationRecipe
    {
        string strCombinationRecipeKey { get; }
        string strRecipeDescription { get; }
    }

    public interface ICombinationMaterial
    {
        string strCombinationMaterialKey { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CombinationChecker : CSingletonDynamicMonoBase<CombinationChecker>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        public void DoInit_CombinationData<TQuestData>(TQuestData[] arrSourceData, params IQuestProgressData[] arrProgressData)
            where TQuestData : IQuestData
        {
            //Dictionary<string, IQuestData> _mapQuestData_Source;
            //Dictionary<string, IQuestProgressData> _mapQuestData_Progress;
            //_mapQuestData.Clear();

            //try
            //{
            //    _mapQuestData_Source = arrSourceData.ToDictionary(p => p.strQuestKey, p => (IQuestData)p);
            //    _mapQuestData_Progress = arrProgressData.ToDictionary(p => p.strQuestKey);
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogError($"{nameof(QuestManager)}-{nameof(DoInit_QuestData)} - Fail - {e}", this);
            //    return;
            //}

            //foreach (var pQuestSource in _mapQuestData_Source.Values)
            //{
            //    string strQuestKey = pQuestSource.strQuestKey;
            //    QuestData pQuestData = new QuestData(pQuestSource);
            //    pQuestData.OnUpdateQuest.Subscribe += OnUpdateQuest_Subscribe;

            //    IQuestProgressData pProgressData;
            //    if (_mapQuestData_Progress.TryGetValue(strQuestKey, out pProgressData))
            //        pQuestData.Event_SetProgress(pProgressData);

            //    _mapQuestData.Add(strQuestKey, pQuestData);
            //}
        }

        public ECombinationResult DoCalculate_Combination()
        {
            return ECombinationResult.Success;
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

        #endregion Private
    }
}