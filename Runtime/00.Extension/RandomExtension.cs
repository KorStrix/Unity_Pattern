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
using System.Linq;

public interface IRandomItem
{
    int IRandomItem_GetPercent();
}

public static class RandomExtension
{
    public static CLASS_RANDOM GetRandomItem<CLASS_RANDOM>(this IEnumerable<CLASS_RANDOM> arrRandomTable)
    {
        int iCount = arrRandomTable.Count();
        if (iCount == 0)
            return default;

        if (iCount == 1)
            return arrRandomTable.First();

        return arrRandomTable.ElementAt(Random.Range(0, iCount));
    }

    /// <summary>
    /// 아직 작업중입니다
    /// </summary>
    /// <typeparam name="CLASS_RANDOM"></typeparam>
    /// <param name="arrRandomTable"></param>
    /// <param name="GetRandomPercentage"></param>
    /// <returns></returns>
    public static CLASS_RANDOM GetRandomItem<CLASS_RANDOM>(this IEnumerable<CLASS_RANDOM> arrRandomTable, System.Func<CLASS_RANDOM, int> GetRandomPercentage)
        where CLASS_RANDOM : class
    {
        int iCount = arrRandomTable.Count();
        if (iCount == 0)
            return null;

        if (iCount == 1)
            return arrRandomTable.First();

        // int iMaxValue = Calculate_MaxValue(arrRandomTable, GetRandomPercentage);
        return arrRandomTable.ElementAt(Random.Range(0, iCount));
    }


    public static CLASS_RANDOM GetRandomItem_ForRandomItem<CLASS_RANDOM>(this IEnumerable<CLASS_RANDOM> arrRandomTable)
        where CLASS_RANDOM : class, IRandomItem
    {
        int iCount = arrRandomTable.Count();
        if (iCount == 0)
            return null;

        if (iCount == 1)
            return arrRandomTable.First();

        CLASS_RANDOM pRandomItem = null;
        int iMaxValue = Calculate_MaxValue(arrRandomTable, p => p.IRandomItem_GetPercent());
        int iRandomValue = Random.Range(0, iMaxValue);
        int iCheckValue = 0;

        foreach(var pRandomItemCurrent in arrRandomTable)
        {
            iCheckValue += pRandomItemCurrent.IRandomItem_GetPercent();
            if (iRandomValue < iCheckValue)
            {
                pRandomItem = pRandomItemCurrent;
                break;
            }
        }

        return pRandomItem;
    }

    static int Calculate_MaxValue<CLASS_RANDOM>(IEnumerable<CLASS_RANDOM> arrRandomTable, System.Func<CLASS_RANDOM, int> GetRandomPercentage)
        where CLASS_RANDOM : class
    {
        int iMaxValue = 0;
        foreach (var pRandomItemCurrent in arrRandomTable)
            iMaxValue += GetRandomPercentage(pRandomItemCurrent);

        return iMaxValue;
    }
}
