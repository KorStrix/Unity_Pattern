#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-14 오후 8:19:02
 *	개요 : 단일 함수만 필요로 하는 옵저버 패턴 래퍼
 *	
 *	C#의 event랑 비슷한 기능인데, 확장되었다고 보시면 됩니다.
 *	
 *	주요 기능
 *	중복 Listener 등록 불가
 *	한번만 구독후 자동으로 구독취소
 *	마지막 이벤트값을 구독할때 바로 갱신?
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;
using System;

#region ObservableCollection

/// <summary>
/// 옵저버 패턴을 쓰기 쉽게 Collection화 하였습니다.
/// </summary>
[Serializable]
public class ObservableCollection
{
    public struct ObserverWrapper
    {
        public Action OnNotify { get; private set; }
        public bool bIsPlayOnce { get; private set; }

        public ObserverWrapper(Action OnNotify, bool bIsPlayOnce = false)
        {
            this.OnNotify = OnNotify; this.bIsPlayOnce = bIsPlayOnce;
        }
    }

    /// <summary>
    /// 현재 리스너 카운트
    /// </summary>
    public int iObserverCount => _mapObserver.Count;

    protected Dictionary<Action, ObserverWrapper> _mapObserver = new Dictionary<Action, ObserverWrapper>();
    protected HashSet<Action> _setRequestRemoveObserver = new HashSet<Action>();

    bool _bIsNotifying;

    /// <summary>
    /// 이벤트를 구독합니다.
    /// </summary>
    public event Action Subscribe
    {
        add => DoRegist_Observer(value);
        remove => DoRemove_Observer(value);
    }

    /// <summary>
    /// 이벤트를 한번만 구독합니다.
    /// </summary>
    public event Action Subscribe_Once
    {
        add => DoRegist_Observer(value, false, true);
        remove => DoRemove_Observer(value);
    }

    /// <summary>
    /// 이벤트를 구독하며 마지막 이벤트값으로 바로 호출합니다.
    /// </summary>
    public event Action Subscribe_And_Listen_CurrentData 
    { 
        add => DoRegist_Observer(value, true);
        remove => DoRemove_Observer(value);
    }

    /// <summary>
    /// 이벤트 리스너들에게 이벤트를 알립니다.
    /// </summary>
    public void DoNotify()
    {
        _bIsNotifying = true;
        foreach (var pListener in _mapObserver.Values)
        {
            if (pListener.OnNotify != null)
                pListener.OnNotify();

            if (pListener.bIsPlayOnce)
                _setRequestRemoveObserver.Add(pListener.OnNotify);
        }

        _bIsNotifying = false;
        if(_setRequestRemoveObserver.Count != 0)
        {
            foreach (var OnNotify in _setRequestRemoveObserver)
                DoRemove_Observer(OnNotify);
            _setRequestRemoveObserver.Clear();
        }
    }

    /// <summary>
    /// 이벤트 리스너들을 삭제합니다.
    /// </summary>
    public void DoClear_Observer()
    {
        _mapObserver.Clear();
    }

    public void DoRegist_Observer(Action OnNotify, bool bInstantNotify_To_ThisListener = false, bool bPlayOnce = false)
    {
        if (OnNotify == null)
            return;

        if (_mapObserver.ContainsKey(OnNotify))
            return;

        if (bInstantNotify_To_ThisListener)
        {
            OnNotify();

            if(bPlayOnce == false)
                _mapObserver.Add(OnNotify, new ObserverWrapper(OnNotify));
        }
        else
        {
            _mapObserver.Add(OnNotify, new ObserverWrapper(OnNotify, bPlayOnce));
        }
    }

    public void DoRemove_Observer(Action OnNotify)
    {
        if (OnNotify == null)
            return;

        if (_bIsNotifying)
        {
            _setRequestRemoveObserver.Add(OnNotify);
            return;
        }

        if (_mapObserver.ContainsKey(OnNotify))
            _mapObserver.Remove(OnNotify);
    }
}

/// <summary>
/// 옵저버 패턴을 쓰기 쉽게 Collection화 하였습니다.
/// </summary>
public class ObservableCollection<Args>
{
    public struct ObserverWrapper
    {
        public Action<Args> OnNotify { get; private set; }
        public bool bIsPlayOnce { get; private set; }

        public ObserverWrapper(Action<Args> OnNotify, bool bIsPlayOnce = false)
        {
            this.OnNotify = OnNotify; this.bIsPlayOnce = bIsPlayOnce;
        }
    }

    public int iObserverCount => _mapObserver.Count;

    public Args pLastArg { get; private set; }

    protected Dictionary<Action<Args>, ObserverWrapper> _mapObserver = new Dictionary<Action<Args>, ObserverWrapper>();
    protected HashSet<Action<Args>> _setRequestRemoveObserver = new HashSet<Action<Args>>();
    bool _bIsNotifying;

    public event Action<Args> Subscribe
    {
        add => DoRegist_Observer(value);
        remove => DoRemove_Observer(value);
    }

    public event Action<Args> Subscribe_Once
    {
        add => DoRegist_Observer(value, false, true);
        remove => DoRemove_Observer(value);
    }

    /// <summary>
    /// 구독과 동시에 마지막 구독 이벤트를 호출합니다
    /// </summary>
    public event Action<Args> Subscribe_And_Listen_CurrentData
    {
        add => DoRegist_Observer(value, true);
        remove => DoRemove_Observer(value);
    }

    /// <summary>
    /// 구독과 동시에 마지막 구독 이벤트를 호출합니다.
    /// <para>그리고 구독을 취소합니다.</para>
    /// </summary>
    public event Action<Args> Subscribe_Once_And_Listen_CurrentData
    {
        add => DoRegist_Observer(value, true, true);
        remove => DoRemove_Observer(value);
    }

    public ObservableCollection()
    {
    }

    public ObservableCollection(Args arg)
    {
        pLastArg = arg;
    }

    public void DoNotify(Args arg)
    {
        _bIsNotifying = true;

        foreach (var pListener in _mapObserver.Values)
        {
            pListener.OnNotify?.Invoke(arg);

            if (pListener.bIsPlayOnce)
                _setRequestRemoveObserver.Add(pListener.OnNotify);
        }

        pLastArg = arg;

        _bIsNotifying = false;
        if (_setRequestRemoveObserver.Count != 0)
        {
            foreach (var pRemoveAction in _setRequestRemoveObserver)
                DoRemove_Observer(pRemoveAction);
            _setRequestRemoveObserver.Clear();
        }
    }

    public void DoClear_Observer()
    {
        _mapObserver.Clear();
    }

    public void DoRegist_Observer(Action<Args> OnNotify, bool bInstantNotify_To_ThisListener = false, bool bPlayOnce = false)
    {
        if (OnNotify == null)
            return;

        if (_mapObserver.ContainsKey(OnNotify))
            return;

        if (bInstantNotify_To_ThisListener)
        {
            OnNotify(pLastArg);

            if (bPlayOnce == false)
                _mapObserver.Add(OnNotify, new ObserverWrapper(OnNotify));
        }
        else
        {
            _mapObserver.Add(OnNotify, new ObserverWrapper(OnNotify, bPlayOnce));
        }
    }

    public void DoRemove_Observer(Action<Args> OnNotify)
    {
        if(_bIsNotifying)
        {
            _setRequestRemoveObserver.Add(OnNotify);
            return;
        }

        if (_mapObserver.ContainsKey(OnNotify))
            _mapObserver.Remove(OnNotify);
    }
}
#endregion ObservableCollection

public class OrderableData<T>
{
    public T TData { get; private set; }
    public int iSortOrder { get; private set; }

    public OrderableData(T Data, int iSortOrder)
    {
        TData = Data;
        this.iSortOrder = iSortOrder;
    }

    public static int Compare_Data_HasOrder(OrderableData<T> x, OrderableData<T> y)
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
        add => DoRegist_Listener(value, 0);

        remove => DoRemove_Listener(value);
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
        add => DoRegist_Listener(value, 0);

        remove => DoRemove_Listener(value);
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