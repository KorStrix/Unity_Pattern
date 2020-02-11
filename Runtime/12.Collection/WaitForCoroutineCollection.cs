#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-27 오전 9:13:16
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1) 코루틴을 리스트에 등록 후 2) 동시에 실행시킨 뒤
/// <para>3) 전부 다 yield break 까지 기다리는 것을 도우는 컬렉션입니다.</para>
/// </summary>
[System.Serializable]
public class WaitForCoroutineCollection
{
    protected HashSet<System.Func<IEnumerator>> _setListener = new HashSet<System.Func<IEnumerator>>();
    protected HashSet<System.Func<IEnumerator>> _setRequestRemoveListener = new HashSet<System.Func<IEnumerator>>();
    protected List<Coroutine> _listWait = new List<Coroutine>();

    bool _bIsNotifying;

    public event System.Func<IEnumerator> Subscribe
    {
        add
        {
            DoRegist_Listener(value);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

    public IEnumerator DoNotify_And_WaitCoroutine(System.Func<IEnumerator, Coroutine> OnStartCoroutine, System.Action<Coroutine> OnStopCoroutine)
    {
        _bIsNotifying = true;

        for (int i = 0; i < _listWait.Count; i++)
            OnStopCoroutine(_listWait[i]);
        _listWait.Clear();
        foreach (var pAction in _setListener)
            _listWait.Add(OnStartCoroutine(pAction()));

        if(_listWait.Count != 0)
            yield return _listWait.GetEnumerator();
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
        _setListener.Clear();
    }

    public void DoRegist_Listener(System.Func<IEnumerator> OnNotify)
    {
        if (OnNotify == null)
            return;

        if (_setListener.Contains(OnNotify) == false)
            _setListener.Add(OnNotify);
    }

    public void DoRemove_Listener(System.Func<IEnumerator> OnNotify)
    {
        if (_bIsNotifying)
        {
            _setRequestRemoveListener.Add(OnNotify);
            return;
        }

        if (_setListener.Contains(OnNotify))
            _setListener.Remove(OnNotify);
    }
}