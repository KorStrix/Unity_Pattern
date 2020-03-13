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
using System.Text;
using System.Linq;

/// <summary>
/// 
/// </summary>
public static class PrimitiveExtension
{
    static public IEnumerable<T> ForEachCustom<T>(this IEnumerable<T> arrTarget, System.Action<T> OnExecute)
    {
        if (arrTarget == null)
            return null;

        foreach (var pTarget in arrTarget)
            OnExecute.Invoke(pTarget);

        return arrTarget;
    }


    static public void Add_IgnoreContain<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pAddKey, TValue pAddValue)
    {
        if (mapTarget.ContainsKey(pAddKey))
            mapTarget[pAddKey] = pAddValue;
        else
            mapTarget.Add(pAddKey, pAddValue);
    }

    static public float Normailize(this float fCurrent_0_1, float fMax, float fMin = 0f)
    {
        return (fCurrent_0_1 * (fMax - fMin)) + fMin;
    }

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

    static public bool TryGetKey_IfContain<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TValue pValue, out TKey pKey)
    {
        bool bResult = mapTarget.ContainsValue(pValue);
        if(bResult)
            pKey = mapTarget.Where(p => p.Value.Equals(pValue)).First().Key;
        else
            pKey = default(TKey);

        return bResult;
    }

    static public TKey GetKey<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TValue pValue)
    {
        bool bResult = mapTarget.ContainsValue(pValue);
        if (bResult)
            return mapTarget.Where(p => p.Value.Equals(pValue)).First().Key;
        else
            return default(TKey);
    }

    static StringBuilder _pBuilder = new StringBuilder();
    static public string ToString_Collection<T>(this IEnumerable<T> arrPrintCollection)
        where T : Component
    {
        _pBuilder.Length = 0;

        _pBuilder.Append("Count : ");
        _pBuilder.Append(arrPrintCollection.Count());

        _pBuilder.Append(" {");
        foreach (var pItem in arrPrintCollection)
        {
            _pBuilder.Append(pItem.name);
            _pBuilder.Append(", ");
        }
        _pBuilder.Length -= 2;
        _pBuilder.Append("}");

        return _pBuilder.ToString();
    }
}
