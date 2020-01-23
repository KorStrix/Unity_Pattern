#region Header
/*	============================================
 *	Aurthor 			    : Strix
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

static public class RandomExtension
{
    static public CLASS_RANDOM GetRandomItem<CLASS_RANDOM>(this IEnumerable<CLASS_RANDOM> arrRandomTable)
        where CLASS_RANDOM : class
    {
        int iCount = arrRandomTable.Count();
        if (iCount == 0)
            return null;

        if (iCount == 1)
            return arrRandomTable.First();

        CLASS_RANDOM pRandomItem = null;
        int iRandomIndex = Random.Range(0, iCount);
        int iCurrentIndex = 0;
        foreach (var pRandomItemCurrent in arrRandomTable)
        {
            if (iRandomIndex == iCurrentIndex++)
            {
                pRandomItem = pRandomItemCurrent;
                break;
            }
        }

        return pRandomItem;
    }

    static public CLASS_RANDOM GetRandomItem_ForRandomItem<CLASS_RANDOM>(this IEnumerable<CLASS_RANDOM> arrRandomTable)
        where CLASS_RANDOM : class, IRandomItem
    {
        int iCount = arrRandomTable.Count();
        if (iCount == 0)
            return null;

        if (iCount == 1)
            return arrRandomTable.First();

        CLASS_RANDOM pRandomItem = null;
        int iMaxValue = Calculate_MaxValue(arrRandomTable);
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

    static int Calculate_MaxValue<CLASS_RANDOM>(IEnumerable<CLASS_RANDOM> arrRandomTable)
        where CLASS_RANDOM : class, IRandomItem

    {
        int iMaxValue = 0;
        foreach (var pRandomItemCurrent in arrRandomTable)
        {
            iMaxValue += pRandomItemCurrent.IRandomItem_GetPercent();
        }

        return iMaxValue;
    }
}