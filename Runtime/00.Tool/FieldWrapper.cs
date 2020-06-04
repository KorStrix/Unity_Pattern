#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-04-02
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 
/// </summary>
public class FieldWrapper<FIELD_TYPE>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public struct ObserverWrapper
	{
		public Action<FIELD_TYPE> OnNotify { get; private set; }
		public Func<FIELD_TYPE, bool> OnCheckCondition { get; private set; }

		public bool bIsPlayOnce { get; private set; }
		public ObserverWrapper(Action<FIELD_TYPE> OnNotify, Func<FIELD_TYPE, bool> OnCheckCondition = null)
		{
			this.OnNotify = OnNotify;
			bIsPlayOnce = false;

			if (OnCheckCondition == null)
				this.OnCheckCondition = Default_CheckCondition;
			else
				this.OnCheckCondition = OnCheckCondition;
		}

		public ObserverWrapper(Action<FIELD_TYPE> OnNotify, bool bIsPlayOnce, Func<FIELD_TYPE, bool> OnCheckCondition = null)
		{
			this.OnNotify = OnNotify;
			this.bIsPlayOnce = bIsPlayOnce;

			if (OnCheckCondition == null)
				this.OnCheckCondition = Default_CheckCondition;
			else
				this.OnCheckCondition = OnCheckCondition;
		}

		public bool DoNotify_Check_IsTrue(FIELD_TYPE Value)
		{
			bool bIsCondition = OnCheckCondition(Value);
			if (bIsCondition)
				OnNotify(Value);

			return bIsCondition;
		}

		static bool Default_CheckCondition(FIELD_TYPE Value)
		{
			return true;
		}
	}

	/* public - Field declaration               */

	public FIELD_TYPE Value
	{
		get => _Value;

		set
		{
			_Value = value;

			DoNotify();
		}
	}

	public event Action<FIELD_TYPE> Subscribe
	{
		add
		{
			DoAddObserver(value);
		}

		remove
		{
			if (value == null)
				return;

			if (_bIsNotifying)
			{
				_setRequestRemoveObserver.Add(value);
				return;
			}

			_mapObserver.Remove(value);
		}
	}

	public int iObserverCount => _mapObserver.Count;

	/* protected & private - Field declaration  */

	Dictionary<Action<FIELD_TYPE>, ObserverWrapper> _mapObserver = new Dictionary<Action<FIELD_TYPE>, ObserverWrapper>();
	HashSet<Action<FIELD_TYPE>> _setRequestRemoveObserver = new HashSet<Action<FIELD_TYPE>>();

	FIELD_TYPE _Value;
	bool _bIsNotifying;

	// ========================================================================== //

	/* public - [Do~Som��thing] Function 	        */

	public static implicit operator FIELD_TYPE(FieldWrapper<FIELD_TYPE> pTarget) => pTarget.Value;

	public FieldWrapper()
	{
		Value = default(FIELD_TYPE);
	}

	public FieldWrapper(FIELD_TYPE pValue)
	{
		Value = pValue;
	}

	public FieldWrapper(params Action<FIELD_TYPE>[] arrObserver)
	{
		Value = default(FIELD_TYPE);

		foreach (var pObserver in arrObserver)
			_mapObserver.Add(pObserver, new ObserverWrapper(pObserver));
	}

	public FieldWrapper(FIELD_TYPE pValue, params Action<FIELD_TYPE>[] arrObserver)
	{
		Value = pValue;

		foreach (var pObserver in arrObserver)
			_mapObserver.Add(pObserver, new ObserverWrapper(pObserver));
	}

    // ========================================================================== //

	/// <summary>
	/// �ʵ尡 ����� �� Observer���� �˸��ϴ�.
	/// </summary>
	/// <param name="OnNotify">����� �Լ��� True�� �� �� ������ �Լ�</param>
	/// <param name="bIsPlayOnce">�ѹ��� �˸��� ����</param>
	public void DoAddObserver(Action<FIELD_TYPE> OnNotify, bool bIsPlayOnce = false)
	{
		if(OnNotify == null)
		{
			Debug.LogError("OnNotify == null");
			return;
		}

		DoAddObserver_WithCondition(null, OnNotify, bIsPlayOnce);
	}

	/// <summary>
	/// (�ʵ尡 ����� �� && ����� �Լ��� True�� �� ��) Observer���� �˸��ϴ�.
	/// </summary>
	/// <param name="OnCheckCondition">����� �Լ�</param>
	/// <param name="OnNotify">����� �Լ��� True�� �� �� ������ �Լ�</param>
	/// <param name="bIsPlayOnce">�ѹ��� �˸��� ����</param>
	public void DoAddObserver_WithCondition(Func<FIELD_TYPE, bool> OnCheckCondition, Action<FIELD_TYPE> OnNotify, bool bIsPlayOnce = false)
	{
		if (OnNotify == null)
		{
			Debug.LogError("OnNotify == null");
			return;
		}

		if (_mapObserver.ContainsKey(OnNotify) == false)
			_mapObserver.Add(OnNotify, new ObserverWrapper(OnNotify, bIsPlayOnce, OnCheckCondition));
	}

	/// <summary>
	/// �ʵ� ���������� ������� Observer���� �˸��ϴ�.
	/// </summary>
	public void DoNotify()
	{
		_bIsNotifying = true;
		foreach (var pObserver in _mapObserver.Values)
		{
			if(pObserver.DoNotify_Check_IsTrue(_Value) && pObserver.bIsPlayOnce)
				_setRequestRemoveObserver.Add(pObserver.OnNotify);
		}

		_bIsNotifying = false;
		if (_setRequestRemoveObserver.Count != 0)
		{
			foreach (var OnNotify in _setRequestRemoveObserver)
				Subscribe -= OnNotify;

			_setRequestRemoveObserver.Clear();
		}
	}

	public void DoClearObserver()
	{
		_mapObserver.Clear();
	}

	// ========================================================================== //

	/* protected - [Override & Unity API]       */


	/* protected - [abstract & virtual]         */


	// ========================================================================== //

	#region Private

	#endregion Private
}