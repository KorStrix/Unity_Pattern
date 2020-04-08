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
public static class CollectionExtension
{
    #region IEnumerable

    static public IEnumerable<T> ForEachCustom<T>(this IEnumerable<T> arrTarget, System.Action<T> OnExecute)
    {
        if (arrTarget == null)
            return null;

        foreach (var pTarget in arrTarget)
            OnExecute.Invoke(pTarget);

        return arrTarget;
    }

    static public HashSet<T> ToHashSet<T>(this IEnumerable<T> arrTarget)
    {
        if (arrTarget == null)
            return null;

        return new HashSet<T>(arrTarget);
    }

    static public bool NullOrEmpty<T>(this IEnumerable<T> arrTarget)
    {
        return arrTarget == null || arrTarget.Count() == 0;
    }

    static public IEnumerable<T> Remove<T>(this IEnumerable<T> arrTarget, IEnumerable<T> arrRemove)
    {
        List<T> listForRemove = new List<T>(arrTarget);
        listForRemove.RemoveAll(p => arrRemove.Contains(p));

        return listForRemove;
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



    #endregion IEnumerable


    static public void RemoveRange<T>(this List<T> arrTarget, IEnumerable<T> arrRemove)
    {
        arrTarget.RemoveAll(p => arrRemove.Contains(p));
    }


    #region Dictionary

    static public void Add_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pAddKey, TValue pAddValue, System.Action<string> OnPrintLog = null)
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

    static public bool ContainsKey_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pKey, System.Action<string> OnPrintLog = null)
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

    static public TValue GetValue_Safe<TKey, TValue>(this Dictionary<TKey, TValue> mapTarget, TKey pKey, System.Action<string> OnPrintLog = null)
    {
        if (mapTarget == null)
        {
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}> - Field is null");
            return default(TValue);
        }

        TValue pValue;
        if (mapTarget.TryGetValue(pKey, out pValue) == false)
            OnPrintLog?.Invoke($"Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}>.Not Contain({pKey.ToString()})");

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

    #endregion Dictionary
}
