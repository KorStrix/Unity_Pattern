#region Header
/*	============================================
 *	Author   			  : Strix
 *	Initial Creation Date : 2020-02-17
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{

    public interface IAchievementLogic
    {
        int iAchievementLogic_Order { get; }
        EAchieveProgress Calculate_AchievementProgress(AchievementDataManager.AchievementData pData, EAchieveProgress eCurrentProgress);
    }

    public class AchievementLogic_CountOver : IAchievementLogic
    {
        public int iAchievementLogic_Order { get; private set; }

        public AchievementLogic_CountOver(int iAchievementLogic_Order = 0)
        {
            this.iAchievementLogic_Order = iAchievementLogic_Order;
        }

        public EAchieveProgress Calculate_AchievementProgress(AchievementDataManager.AchievementData pData, EAchieveProgress eCurrentProgress)
        {
            if (eCurrentProgress == EAchieveProgress.Fail || eCurrentProgress == EAchieveProgress.GiveUp)
                return eCurrentProgress;

            if (pData.pProgressData.eAchieveProgress == EAchieveProgress.None && pData.pProgressData.iAchievementCount == 0)
                return EAchieveProgress.None;

            if (pData.pAchievementData.iAchievementCount > pData.pProgressData.iAchievementCount)
                return EAchieveProgress.In_Progress;
            else
                return EAchieveProgress.Success;
        }
    }
}