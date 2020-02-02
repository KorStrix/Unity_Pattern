#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-13 오후 4:37:18
 *	개요 : 
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
    public static class PrimitiveExtension
    {
        static public T ConvertEnum<T>(this string strText)
            where T : struct
        {
            T pEnum;
            if(System.Enum.TryParse(strText, out pEnum) == false)
            {
                Debug.LogError($"Enum Parsing Fail - ({pEnum.GetType()}){strText}");
            }

            return pEnum;
        }

        static public TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pKey, bool bIsPrintError = true)
        {
            TValue pValue;
            if(mapTarget.TryGetValue(pKey, out pValue) == false)
            {
                if(bIsPrintError)
                    Debug.LogError($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}>.Not Contain({pKey.ToString()})");
            }

            return pValue;
        }
    }
}