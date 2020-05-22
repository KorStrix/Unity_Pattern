#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-18 오후 5:25:13
 *	기능 : 
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Key는 Min ~ Max Range로 이루어져 있으며,
/// 특정 값을 넣으면 Range에 해당하는 Value 혹은 Null을 리턴합니다.
/// </summary>
public class RangeDictionary<TKey, TValue> : IDictionary<RangeDictionary<TKey, TValue>.Range, TValue>
    where TKey : IComparable<TKey>
{
    [Serializable]
    public struct Range
    {
        public TKey TKeyMin { get; private set; }
        public TKey TKeyMax { get; private set; }

        public Range(TKey TKeyMin, TKey TKeyMax)
        {
            this.TKeyMin = TKeyMin;
            this.TKeyMax = TKeyMax;
        }
    }

    public int Count => _InDictionary.Count;

    public bool IsReadOnly => false;

    public TValue this[TKey key] => GetValue(key);

    readonly Dictionary<Range, TValue> _InDictionary = new Dictionary<Range, TValue>();
    readonly Dictionary<TKey, Range> _mapAlreadyExist = new Dictionary<TKey, Range>();

    readonly Func<TKey, TKey, int> _OnCompareTo;

    // =================================================================================================

    private static int DefaultCompare(TKey KeyX, TKey KeyY)
    {
        return KeyX.CompareTo(KeyY);
    }

    public RangeDictionary()
    {
        _OnCompareTo = DefaultCompare;
    }

    public RangeDictionary(Func<TKey, TKey, int> OnCompareTo)
    {
        _OnCompareTo = OnCompareTo;
    }

    // =================================================================================================

    public bool Add(TKey Range_Min, TKey Range_Max, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(Range_Min) || _mapAlreadyExist.ContainsKey(Range_Max))
            return false;

        Range sRange = new Range(Range_Min, Range_Max);
        _mapAlreadyExist.Add(Range_Min, sRange);
        _mapAlreadyExist.Add(Range_Max, sRange);
        _InDictionary.Add(sRange, pValue);

        return true;
    }

    public void Add(Range sRange, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(sRange.TKeyMin) || _mapAlreadyExist.ContainsKey(sRange.TKeyMax))
            return;

        _mapAlreadyExist.Add(sRange.TKeyMin, sRange);
        _mapAlreadyExist.Add(sRange.TKeyMax, sRange);
        _InDictionary.Add(sRange, pValue);
    }

    public bool Remove(TKey Range_Min, TKey Range_Max)
    {
        Range sRange = new Range(Range_Min, Range_Max);
        if (_InDictionary.ContainsKey(sRange) == false)
            return false;

        _mapAlreadyExist.Remove(Range_Min);
        _mapAlreadyExist.Remove(Range_Max);
        _InDictionary.Remove(sRange);

        return true;
    }

    public TValue GetValue(TKey tKey)
    {
        foreach (TKey tKeyCurrent in _mapAlreadyExist.Keys)
        {
            Range sRange = _mapAlreadyExist[tKeyCurrent];
            int iCompareCurrent = _OnCompareTo(tKeyCurrent, tKey);
            if (iCompareCurrent == -1) // 비교값이 KeyCurrent보다 크다면
            {
                // Min은 KeyCurrent여야 하고, 비교값은 Max보다 작아야 한다.
                if (_OnCompareTo(sRange.TKeyMin, tKeyCurrent) == 0 && _OnCompareTo(sRange.TKeyMax, tKey) == 1)
                    return _InDictionary[sRange];
            }
            else if (iCompareCurrent == 1) // 비교값이 KeyCurrent보다 작다면
            {
                // Max은 KeyCurrent여야 하고, 비교값은 Max보다 작아야 한다.
                if (_OnCompareTo(sRange.TKeyMin, tKey) == -1 && _OnCompareTo(sRange.TKeyMax, tKeyCurrent) == 0)
                    return _InDictionary[sRange];
            }
            else // 비교값이 KeyCurrent와 같다면 위 조건을 둘다 실행해야 한다.
            {
                if (_OnCompareTo(sRange.TKeyMin, tKeyCurrent) == 0 && _OnCompareTo(sRange.TKeyMax, tKey) == 1)
                    return _InDictionary[sRange];
                if (_OnCompareTo(sRange.TKeyMin, tKey) == -1 && _OnCompareTo(sRange.TKeyMax, tKeyCurrent) == 0)
                    return _InDictionary[sRange];
            }
        }

        return default;
    }

    public void Clear()
    {
        _mapAlreadyExist.Clear();
        _InDictionary.Clear();
    }


    #region IDictionary

    public ICollection<Range> Keys => _InDictionary.Keys;
    public ICollection<TValue> Values => _InDictionary.Values;

    public TValue this[Range key]
    {
        get => _InDictionary[key];
        set => _InDictionary[key] = value;
    }

    public void Add(KeyValuePair<Range, TValue> item)
    {
        _InDictionary.Add(item.Key, item.Value);
    }

    public bool ContainsKey(Range key)
    {
        return _InDictionary.ContainsKey(key);
    }

    public bool Contains(KeyValuePair<Range, TValue> item)
    {
        return _InDictionary.Contains(item);
    }

    public bool Remove(Range key)
    {
        return _InDictionary.Remove(key);
    }

    public bool Remove(KeyValuePair<Range, TValue> item)
    {
        return _InDictionary.Remove(item.Key);
    }

    public bool TryGetValue(Range key, out TValue value)
    {
        return _InDictionary.TryGetValue(key, out value);
    }

    public void CopyTo(KeyValuePair<Range, TValue>[] array, int arrayIndex)
    {
        var entries = _InDictionary.GetEnumerator();
        while(entries.MoveNext())
        {
            var pCurrent = entries.Current;
            array[arrayIndex++] = new KeyValuePair<Range, TValue>(pCurrent.Key, pCurrent.Value);
        }
        entries.Dispose();
    }

    public IEnumerator<KeyValuePair<Range, TValue>> GetEnumerator()
    {
        return _InDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion IDictionary
}