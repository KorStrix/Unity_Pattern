#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-13 오후 4:37:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// 
/// </summary>
public static class CollectionExtension
{
    #region IEnumerable

    public static IEnumerable<T> ForEachCustom<T>(this IEnumerable<T> arrTarget, System.Action<T> OnExecute)
    {
        if (arrTarget == null)
            return null;

        foreach (var pTarget in arrTarget)
            OnExecute.Invoke(pTarget);

        return arrTarget;
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> arrTarget)
    {
        if (arrTarget == null)
            return null;

        return new HashSet<T>(arrTarget);
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> arrTarget)
    {
        return arrTarget == null || arrTarget.Any() == false;
    }

    public static IEnumerable<T> Remove<T>(this IEnumerable<T> arrTarget, IEnumerable<T> arrRemove)
    {
        List<T> listForRemove = new List<T>(arrTarget);
        listForRemove.RemoveAll(arrRemove.Contains);

        return listForRemove;
    }

    static readonly StringBuilder _pBuilder = new StringBuilder();
    public static string ToString_Collection<T>(this IEnumerable<T> arrPrintCollection)
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

    public static IEnumerable<T> RemoveAt_Custom<T>(this IEnumerable<T> arrPrintCollection, int iStartIndex, int iCount)
    {
        List<T> listTemp = arrPrintCollection.ToList();
        listTemp.RemoveRange(iStartIndex, iCount);

        return listTemp;
    }

    public static T GetValue_OrDefault<T>(this T[] array, int iIndex, System.Action<string> OnFail = null)
    {
        if (array == null)
        {
            OnFail?.Invoke($"{nameof(GetValue_OrDefault)} array == null");
            return default(T);
        }

        if (array.Length < iIndex)
        {
            OnFail?.Invoke($"{nameof(GetValue_OrDefault)} array.Length({array.Length}) < iIndex({iIndex})");
            return default(T);
        }

        return array[iIndex];
    }

    #endregion IEnumerable


    #region List

    public static bool TryGetValue_Safe<T>(this List<T> arrTarget, int iIndex, out T pValue)
    {
        pValue = default(T);
        
        if (arrTarget.IsNullOrEmpty())
            return false;

        if (iIndex < 0 || iIndex >= arrTarget.Count)
            return false;

        pValue = arrTarget[iIndex];
        
        return true;
    }
    
    public static List<T> RemoveRange<T>(this List<T> arrTarget, IEnumerable<T> arrRemove)
    {
        arrTarget.RemoveAll(arrRemove.Contains);

        return arrTarget;
    }

    public static List<T> OrderByCustom<T>(this List<T> arrTarget, System.Func<T, int> OnGetOrder)
    {
        if (OnGetOrder == null)
        {
            Debug.LogError($"{nameof(OrderByCustom)} OnGetOrder == null");
            return arrTarget;
        }

        arrTarget.Sort((x, y) => OnGetOrder(x).CompareTo(OnGetOrder(y)));
        return arrTarget;
    }

    #endregion


    #region Dictionary

    public static void Add_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pAddKey, TValue pAddValue) => mapTarget.Add_Safe(pAddKey, pAddValue, Debug.Log);

    public static void Add_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pAddKey, TValue pAddValue, System.Action<string> OnPrintLog)
    {
        if (mapTarget == null)
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Field is null");
            return;
        }

        if (mapTarget.ContainsKey(pAddKey))
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - {nameof(Add_Safe)} - Already Contain key : {pAddKey.ToString()}");
            mapTarget[pAddKey] = pAddValue;
        }
        else
            mapTarget.Add(pAddKey, pAddValue);
    }

    public static bool ContainsKey_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pKey) => mapTarget.ContainsKey_Safe(pKey, Debug.Log);
    public static bool ContainsKey_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pKey, System.Action<string> OnPrintLog)
    {
        if (mapTarget == null)
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Field is null");
            return false;
        }

        if(pKey == null)
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Key is null");
            return false;
        }

        return mapTarget.ContainsKey(pKey);
    }

    public static TValue GetValue_OrDefault<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey tKey) =>  mapTarget.GetValue_OrDefault(tKey, Debug.Log);

    public static TValue GetValue_OrDefault<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey tKey, System.Action<string> OnPrintLog)
    {
        if (mapTarget == null)
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Field is null");
            return default(TValue);
        }

        if (mapTarget.Count == 0)
            return default(TValue);

        if (mapTarget.TryGetValue(tKey, out var pValue) == false)
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}>.Not Contain({tKey})");

        return pValue;
    }

    public static bool TryGetValue_Custom<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey tKey, out TValue tValue, System.Action<string> OnPrintLog)
    {
        if (mapTarget == null)
        {
            tValue = default(TValue);
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Field is null");
            return false;
        }

        bool bResult = mapTarget.TryGetValue(tKey, out tValue);
        if(bResult == false)
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}>.Not Contain({tKey})");

        return bResult;
    }

    public static bool TryGetKey_IfContain<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TValue tValue, out TKey tKey)
    {
        bool bResult = mapTarget.ContainsValue(tValue);
        tKey = bResult ? mapTarget.First(p => p.Value.Equals(tValue)).Key : default(TKey);

        return bResult;
    }

    public static TKey GetKey<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TValue pValue)
    {
        bool bResult = mapTarget.ContainsValue(pValue);
        return bResult ? mapTarget.First(p => p.Value.Equals(pValue)).Key : default(TKey);
    }

    #endregion Dictionary
}
