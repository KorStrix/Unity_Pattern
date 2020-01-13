#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-14 오후 8:19:02
 *	개요 : 단일 함수만 필요로 하는 옵저버 패턴 래퍼
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region ObservableCollection

/// <summary>
/// 옵저버 패턴을 쓰기 쉽게 Collection화 하였습니다.
/// </summary>
[System.Serializable]
public class ObservableCollection
{
    public struct ListenerWrapper
    {
        public System.Action OnNotify { get; private set; }
        public bool bIsPlayOnce { get; private set; }

        public ListenerWrapper(System.Action OnNotify, bool bIsPlayOnce = false)
        {
            this.OnNotify = OnNotify; this.bIsPlayOnce = bIsPlayOnce;
        }
    }

    protected Dictionary<System.Action, ListenerWrapper> _mapListener = new Dictionary<System.Action, ListenerWrapper>();
    protected HashSet<System.Action> _setRequestRemoveListener = new HashSet<System.Action>();

    bool _bIsNotifying;

    public event System.Action Subscribe
    {
        add { DoRegist_Listener(value); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action Subscribe_Once
    {
        add { DoRegist_Listener(value, false, true); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action Subscribe_And_Listen_CurrentData 
    { 
        add { DoRegist_Listener(value, true); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action Subscribe_Once_And_Listen_CurrentData
    {
        add { DoRegist_Listener(value, true, true); }
        remove { DoRemove_Listener(value); }
    }

    public void DoNotify()
    {
        _bIsNotifying = true;
        foreach (var pListener in _mapListener.Values)
        {
            if (pListener.OnNotify != null)
                pListener.OnNotify();

            if (pListener.bIsPlayOnce)
                _setRequestRemoveListener.Add(pListener.OnNotify);
        }

        _bIsNotifying = false;
        if(_setRequestRemoveListener.Count != 0)
        {
            foreach (var OnNotify in _setRequestRemoveListener)
                DoRemove_Listener(OnNotify);
            _setRequestRemoveListener.Clear();
        }
    }

    public void DoClear_Listener()
    {
        _mapListener.Clear();
    }

    public void DoRegist_Listener(System.Action OnNotify, bool bInstantNotify_To_ThisListener = false, bool bPlayOnce = false)
    {
        if (OnNotify == null)
            return;

        if (_mapListener.ContainsKey(OnNotify))
            return;

        if (bInstantNotify_To_ThisListener)
        {
            OnNotify();

            if(bPlayOnce == false)
                _mapListener.Add(OnNotify, new ListenerWrapper(OnNotify, bPlayOnce));
        }
        else
        {
            _mapListener.Add(OnNotify, new ListenerWrapper(OnNotify, bPlayOnce));
        }
    }

    public void DoRemove_Listener(System.Action OnNotify)
    {
        if (OnNotify == null)
            return;

        if (_bIsNotifying)
        {
            _setRequestRemoveListener.Add(OnNotify);
            return;
        }

        if (_mapListener.ContainsKey(OnNotify))
            _mapListener.Remove(OnNotify);
    }
}

/// <summary>
/// 옵저버 패턴을 쓰기 쉽게 Collection화 하였습니다.
/// </summary>
public class ObservableCollection<Args>
{
    public struct ListenerWrapper
    {
        public System.Action<Args> OnNotify { get; private set; }
        public bool bIsPlayOnce { get; private set; }

        public ListenerWrapper(System.Action<Args> OnNotify, bool bIsPlayOnce = false)
        {
            this.OnNotify = OnNotify; this.bIsPlayOnce = bIsPlayOnce;
        }
    }

    [NonSerialized]
    private Args _LastArg; public Args GetLastArg_1() { return _LastArg; }

    protected Dictionary<System.Action<Args>, ListenerWrapper> _mapListener = new Dictionary<Action<Args>, ListenerWrapper>();
    protected HashSet<System.Action<Args>> _setRequestRemoveListener = new HashSet<System.Action<Args>>();
    bool _bIsNotifying;

    public event System.Action<Args> Subscribe
    {
        add { DoRegist_Listener(value); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action<Args> Subscribe_Once
    {
        add { DoRegist_Listener(value, false, true); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action<Args> Subscribe_And_Listen_CurrentData
    {
        add { DoRegist_Listener(value, true); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action<Args> Subscribe_Once_And_Listen_CurrentData
    {
        add { DoRegist_Listener(value, true, true); }
        remove { DoRemove_Listener(value); }
    }

    public void DoNotify(Args arg)
    {
        _bIsNotifying = true;

        foreach (var pListener in _mapListener.Values)
        {
            if(pListener.OnNotify != null)
                pListener.OnNotify(arg);

            if (pListener.bIsPlayOnce)
                _setRequestRemoveListener.Add(pListener.OnNotify);
        }

        _LastArg = arg;

        _bIsNotifying = false;
        if (_setRequestRemoveListener.Count != 0)
        {
            foreach (var pRemoveAction in _setRequestRemoveListener)
                DoRemove_Listener(pRemoveAction);
            _setRequestRemoveListener.Clear();
        }
    }

    public void DoClear_Listener()
    {
        _mapListener.Clear();
    }

    public void DoRegist_Listener(System.Action<Args> OnNotify, bool bInstantNotify_To_ThisListener = false, bool bPlayOnce = false)
    {
        if (OnNotify == null)
            return;

        if (_mapListener.ContainsKey(OnNotify))
            return;

        if (bInstantNotify_To_ThisListener)
        {
            OnNotify(_LastArg);

            if (bPlayOnce == false)
                _mapListener.Add(OnNotify, new ListenerWrapper(OnNotify, bPlayOnce));
        }
        else
        {
            _mapListener.Add(OnNotify, new ListenerWrapper(OnNotify, bPlayOnce));
        }
    }

    public void DoRemove_Listener(System.Action<Args> OnNotify)
    {
        if(_bIsNotifying)
        {
            _setRequestRemoveListener.Add(OnNotify);
            return;
        }

        if (_mapListener.ContainsKey(OnNotify))
            _mapListener.Remove(OnNotify);
    }
}
#endregion ObservableCollection

public class OrderableData<T>
{
    public T TData { get; private set; }
    public int iSortOrder { get; private set; }

    public OrderableData(T Data, int iSortOrder)
    {
        this.TData = Data;
        this.iSortOrder = iSortOrder;
    }

    static public int Compare_Data_HasOrder(OrderableData<T> x, OrderableData<T> y)
    {
        return x.iSortOrder.CompareTo(y.iSortOrder);
    }
}


#region ObservableCollection_RefData

/// <summary>
/// 옵저버 패턴 응용버전, 이벤트 발생시 옵저버들의 계산 결과를 리턴합니다.
/// </summary>
public class ObservableCollection_RefData<TResultData>
{
    public delegate void OnChainData(TResultData pValue_Origin, ref TResultData pValue_Current);

    [NonSerialized]
    private TResultData _LastChainData_Origin; public TResultData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TResultData _LastChainData_Current; public TResultData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, OrderableData<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, OrderableData<OnChainData>>();
    protected List<OrderableData<OnChainData>> _listListener = new List<OrderableData<OnChainData>>();
    protected HashSet<OnChainData> _setRequestRemoveListener = new HashSet<OnChainData>();

    bool _bIsNotifying;

    public event OnChainData Subscribe
    {
        add
        {
            DoRegist_Listener(value, 0);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

    public TResultData DoNotify(TResultData pValue)
    {
        _bIsNotifying = true;

        TResultData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        _bIsNotifying = false;
        if (_setRequestRemoveListener.Count != 0)
        {
            foreach (var pRemoveAction in _setRequestRemoveListener)
                DoRemove_Listener(pRemoveAction);
            _setRequestRemoveListener.Clear();
        }

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(TResultData arg)
    {
        _bIsNotifying = true;

        TResultData pOrigin = arg;
        for (int i = 0; i < _listListener.Count; i++)
        {
            OrderableData<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(pOrigin, ref arg);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + " Origin : " + pOrigin + " Current : " + arg);
        }

        _bIsNotifying = false;
        if (_setRequestRemoveListener.Count != 0)
        {
            foreach (var pRemoveAction in _setRequestRemoveListener)
                DoRemove_Listener(pRemoveAction);
            _setRequestRemoveListener.Clear();
        }

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = arg;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        OrderableData<OnChainData> pDeleagateInstance = new OrderableData<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(OrderableData<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if(_bIsNotifying)
        {
            _setRequestRemoveListener.Add(OnNotify);
            return;
        }

        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        OrderableData<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}

/// <summary>
/// 옵저버 패턴 응용버전, 이벤트 발생시 옵저버들의 계산 결과를 리턴합니다.
/// </summary>
public class ObservableCollection_RefData<Args, TResultData>
{
    public delegate void OnChainData(Args arg1, TResultData pValue_Origin, ref TResultData pValue_Current);

    [NonSerialized]
    private Args _LastArg; public Args GetLastArg() { return _LastArg; }

    [NonSerialized]
    private TResultData _LastChainData_Origin; public TResultData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TResultData _LastChainData_Current; public TResultData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, OrderableData<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, OrderableData<OnChainData>>();
    protected List<OrderableData<OnChainData>> _listListener = new List<OrderableData<OnChainData>>();

    public event OnChainData Subscribe
    {
        add
        {
            DoRegist_Listener(value, 0);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

    public TResultData DoNotify(Args arg, TResultData pValue)
    {
        TResultData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(arg, pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(Args arg, TResultData pValue)
    {
        TResultData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
        {
            OrderableData<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(arg, pOrigin, ref pValue);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + "arg : " + arg + " Origin : " + pOrigin + " Current : " + pValue);
        }

        _LastArg = arg;
        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        OrderableData<OnChainData> pDeleagateInstance = new OrderableData<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(OrderableData<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        OrderableData<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}
#endregion ObservableCollection_ChainData