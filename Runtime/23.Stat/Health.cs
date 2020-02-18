#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-12 오후 12:06:10
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
    public class Health : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EHealthEvent
        {
            None = 0,

            Reset,
            Recovery,
            Damaged,
            Dead,
        }

        /* public - Field declaration            */

        public delegate void delOnHealthEvent(EHealthEvent eHealthEvent, int iHP_MAX, int iHP_Current, int iChangedHP, float fHP_0_1);
        public event delOnHealthEvent OnHealthEvent;

        public int iHP => _iHP_Current;
        public bool bIsAlive => _iHP_Current > 0;
        public bool bIsDamaged => _iHP_Current < _iHP_MAX;

        /* protected & private - Field declaration         */

        [SerializeField]
        int _iHP_MAX;

        [SerializeField]
        int _iHP_Current;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoClear_HealthEvent()
        {
            OnHealthEvent = null;
        }

        public void DoSet(int iHP)
        {
            _iHP_MAX = iHP;
            OnHealthEvent?.Invoke(EHealthEvent.None, _iHP_MAX, _iHP_Current, 0, GetHP_0_1());
        }

        public void DoReset()
        {
            _iHP_Current = _iHP_MAX;
            OnHealthEvent?.Invoke(EHealthEvent.Reset, _iHP_MAX, _iHP_Current, 0, 1);
        }

        public void DoDamage(int iDamageAmount)
        {
            if (_iHP_Current <= 0)
                return;

            if (_iHP_Current > iDamageAmount)
            {
                _iHP_Current -= iDamageAmount;
                OnHealthEvent?.Invoke(EHealthEvent.Damaged, _iHP_MAX, _iHP_Current, iDamageAmount, GetHP_0_1());
            }
            else
            {
                _iHP_Current = 0;
                OnHealthEvent?.Invoke(EHealthEvent.Dead, _iHP_MAX, _iHP_Current, iDamageAmount, 0);
            }
        }

        public void DoRecovery(int iRecoveryAmount)
        {
            if (_iHP_Current == _iHP_MAX)
                return;

            _iHP_Current += iRecoveryAmount;
            if (_iHP_Current > _iHP_MAX)
                _iHP_Current = _iHP_MAX;

            OnHealthEvent?.Invoke(EHealthEvent.Recovery, _iHP_MAX, _iHP_Current, iRecoveryAmount, GetHP_0_1());
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnEnableObject()
        {
            base.OnEnableObject();

            DoReset();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private float GetHP_0_1()
        {
            return _iHP_Current / (float)_iHP_MAX;
        }

        #endregion Private
    }
}