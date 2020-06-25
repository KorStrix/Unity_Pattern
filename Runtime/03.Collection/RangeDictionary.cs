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

[Serializable]
public struct Range<T> : IComparable<T>, IComparable
    where T : IComparable<T>
{
    public static implicit operator Range<T>(T Value) => new Range<T>(Value);

    public T Min { get; private set; }
    public T Max { get; private set; }

    public Range(T Same_Min_And_Max)
    {
        this.Min = Same_Min_And_Max;
        this.Max = Same_Min_And_Max;
    }

    public Range(T Min, T Max)
    {
        this.Min = Min;
        this.Max = Max;
    }

    public static int CompareTo(Range<T> Range, T other)
    {
        int iCompareResult_Min = Range.Min.CompareTo(other);

        // 최소값보다 작을때는 작다.
        if (iCompareResult_Min == -1)
            return -1;

        int iCompareResult_Max = Range.Max.CompareTo(other);

        // 최대값보다 클때는 크다.
        if (iCompareResult_Max == 1)
            return 1;

        // 여기에 진입한 경우는
        // 최소값보다 같거나 클때 && 최대값보다 같거나 작을때이다.
        return 0;
    }

    public int CompareTo(T other)
    {
        return CompareTo(this, other);
    }

    public override string ToString()
    {
        return $"{Min}~{Max}";
    }

    public int CompareTo(object pObject)
    {
        if (pObject is T TValue)
            return CompareTo(this, TValue);

        if (pObject is Range<T> sOther)
        {
            int iMinResult = this.Min.CompareTo(sOther.Min);
            if (iMinResult != 0)
                return iMinResult;

            return this.Max.CompareTo(sOther.Max);
        }

        throw new ArgumentException($"Object is not a {nameof(Range<T>)} And {nameof(T)}");
    }

    public class Comparer : EqualityComparer<Range<T>>
    {
        public override bool Equals(Range<T> x, Range<T> y)
        {
            int iCompareResult_Min = x.Min.CompareTo(y.Min);
            int iCompareResult_Max = x.Max.CompareTo(y.Max);

            return iCompareResult_Min <= 0  // x의 min이 y의 min보다 크거나 같을 때
                   && 
                   0 <= iCompareResult_Max; // x의 max가 y의 max보다 작거나 같을 때
        }

        public override int GetHashCode(Range<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}

/// <summary>
/// Key는 Min ~ Max Range로 이루어져 있으며,
/// 특정 값을 넣으면 Range에 해당하는 Value 혹은 Null을 리턴합니다.
/// <para>Key : 1~10 Value : Object1</para>
/// <para>Key : 11~20 Value : Object2</para>
/// </summary>
public class RangeDictionary<TKey, TValue> : IDictionary<Range<TKey>, TValue>
    where TKey : IComparable<TKey>
{
    public int Count => _InDictionary.Count;

    public bool IsReadOnly => false;

    public TValue this[TKey key] => GetValue(key);

    readonly Dictionary<Range<TKey>, TValue> _InDictionary = new Dictionary<Range<TKey>, TValue>();
    readonly Dictionary<TKey, Range<TKey>> _mapAlreadyExist = new Dictionary<TKey, Range<TKey>>();

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

    public RangeDictionary(Func<TKey, TKey, int> OnCompareTo_Custom)
    {
        _OnCompareTo = OnCompareTo_Custom;
    }

    // =================================================================================================

    public bool Add(TKey Range_Min, TKey Range_Max, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(Range_Min) || _mapAlreadyExist.ContainsKey(Range_Max))
            return false;

        Range<TKey> sRange = new Range<TKey>(Range_Min, Range_Max);
        _mapAlreadyExist.Add(Range_Min, sRange);
        _mapAlreadyExist.Add(Range_Max, sRange);
        _InDictionary.Add(sRange, pValue);

        return true;
    }

    public void Add(Range<TKey> sRange, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(sRange.Min) || _mapAlreadyExist.ContainsKey(sRange.Max))
            return;

        _mapAlreadyExist.Add(sRange.Min, sRange);
        _mapAlreadyExist.Add(sRange.Max, sRange);
        _InDictionary.Add(sRange, pValue);
    }

    public bool Remove(TKey Range_Min, TKey Range_Max)
    {
        Range<TKey> sRange = new Range<TKey>(Range_Min, Range_Max);
        if (_InDictionary.ContainsKey(sRange) == false)
            return false;

        _mapAlreadyExist.Remove(Range_Min);
        _mapAlreadyExist.Remove(Range_Max);
        _InDictionary.Remove(sRange);

        return true;
    }

    public TValue GetValue(TKey tKey)
    {
        TryGetValue(tKey, out var pValue);

        return pValue;
    }

    public bool TryGetValue(TKey tKey, out TValue pValue)
    {
        foreach (Range<TKey> sRange in _InDictionary.Keys)
        {
            if (_OnCompareTo(sRange.Min, tKey) <= 0 && _OnCompareTo(tKey, sRange.Max) <= 0)
            {
                pValue = _InDictionary[sRange];
                return true;
            }
        }

        pValue = default;
        return false;
    }

    /// <summary>
    /// Key값보다 같지 않고 작은 Key의 데이터만 얻어오기를 시도합니다.
    /// </summary>
    public bool TryGetValue_LesserThenKey(TKey tKey, out TValue pValue)
    {
        if (_InDictionary.Count == 0)
        {
            pValue = default;
            return false;
        }

        // Dictionary의 범위의 최대값보다 클을 경우 최대값을 리턴
        Range<TKey> sRangeMax = _InDictionary.Keys.Max();
        if (tKey.CompareTo(sRangeMax.Max) > 0)
            return TryGetValue(sRangeMax.Max, out pValue);

        // 내림차순으로 루프하며 범위의 최대값보다 큰 경우
        // 찾고자 하는 값
        Range<TKey>[] arrDictionaryKeys = _InDictionary.Keys.Reverse().ToArray();
        foreach (Range<TKey> sRange in arrDictionaryKeys)
        {
            // 타겟 키 > 범위의 최대값이면
            if (_OnCompareTo(tKey, sRange.Max) > 0)
                return _InDictionary.TryGetValue(sRange, out pValue);
        }

        pValue = default;
        return false;
    }


    /// <summary>
    /// Key값보다 같지 않고 큰 Key의 데이터만 얻어오기를 시도합니다.
    /// </summary>
    public bool TryGetValue_GreaterThenKey(TKey tKey, out TValue pValue)
    {
        if (_InDictionary.Count == 0)
        {
            pValue = default;
            return false;
        }

        // Dictionary의 범위의 최소값보다 작을 경우 최소값을 리턴
        Range<TKey> sRangeMin = _InDictionary.Keys.Min();
        if (tKey.CompareTo(sRangeMin.Min) < 0)
            return TryGetValue(sRangeMin.Min, out pValue);

        // 오름차순으로 루프하며 범위의 최소값보다 작으면
        // 찾고자 하는 값
        Range<TKey>[] arrDictionaryKeys = _InDictionary.Keys.ToArray();
        foreach (Range<TKey> sRange in arrDictionaryKeys)
        {
            // 타겟 키 < 범위의 최소값이면
            if (_OnCompareTo(tKey, sRange.Min) < 0)
                return _InDictionary.TryGetValue(sRange, out pValue);
        }

        pValue = default;
        return false;
    }

    public void Clear()
    {
        _mapAlreadyExist.Clear();
        _InDictionary.Clear();
    }


    #region IDictionary

    public ICollection<Range<TKey>> Keys => _InDictionary.Keys;
    public ICollection<TValue> Values => _InDictionary.Values;

    public TValue this[Range<TKey> key]
    {
        get => _InDictionary[key];
        set => _InDictionary[key] = value;
    }


    public bool ContainsKey(Range<TKey> key)
    {
        if (TryGetValue(key.Min, out _))
            return true;

        if (TryGetValue(key.Max, out _))
            return true;

        return false;
    }

    public void Add(KeyValuePair<Range<TKey>, TValue> item) => _InDictionary.Add(item.Key, item.Value);

    public bool Contains(KeyValuePair<Range<TKey>, TValue> item) =>_InDictionary.Contains(item);

    public bool Remove(Range<TKey> key) => _InDictionary.Remove(key);

    public bool Remove(KeyValuePair<Range<TKey>, TValue> item) => _InDictionary.Remove(item.Key);

    public bool TryGetValue(Range<TKey> key, out TValue value) => _InDictionary.TryGetValue(key, out value);

    public void CopyTo(KeyValuePair<Range<TKey>, TValue>[] array, int arrayIndex)
    {
        var entries = _InDictionary.GetEnumerator();
        while(entries.MoveNext())
        {
            var pCurrent = entries.Current;
            array[arrayIndex++] = new KeyValuePair<Range<TKey>, TValue>(pCurrent.Key, pCurrent.Value);
        }
        entries.Dispose();
    }

    public IEnumerator<KeyValuePair<Range<TKey>, TValue>> GetEnumerator() => _InDictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion IDictionary

}