#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-10-27
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using System;
using System.Collections.Generic;
using Logic;
using Unity_Pattern;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 시작 시간을 가지고 있는지 유무
    /// </summary>
    public interface IHasStartDateTime
    {
        DateTime GetStartDateTime();
    }

    /// <summary>
    /// 끝날 시간을 가지고 있는지 유무
    /// </summary>
    public interface IHasEndDateTime
    {
        DateTime GetEndDateTime();
    }

    public interface IHasPeriod : IHasStartDateTime, IHasEndDateTime
    {

    }
}

public static class HasDateTime_Helper
{
    public static bool Check_IsWithinPeriod(this IHasEndDateTime iTarget, DateTime sNowDateTime)
    {
        return sNowDateTime <= iTarget.GetEndDateTime();
    }

    public static bool Check_IsWithinPeriod(this IHasStartDateTime iTarget, DateTime sNowDateTime)
    {
        return iTarget.GetStartDateTime() <= sNowDateTime;
    }

    public static bool Check_IsWithinPeriod<T>(this T iTarget, DateTime sNowDateTime)
        where T : IHasStartDateTime, IHasEndDateTime
    {
        return iTarget.GetStartDateTime() <= sNowDateTime && sNowDateTime <= iTarget.GetEndDateTime();
    }
}