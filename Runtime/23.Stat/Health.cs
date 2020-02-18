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
    /// 유닛의 체력을 담당하는 클래스
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

        /// <summary>
        /// 체력 변동 이벤트가 생길때
        /// </summary>
        public event delOnHealthEvent OnHealthEvent;

        /// <summary>
        /// 현재 체력
        /// </summary>
        public int iHP => _iHP_Current;

        /// <summary>
        /// 현재 살아있는지
        /// </summary>
        public bool bIsAlive => _iHP_Current > 0;

        /// <summary>
        /// 체력이 최대체력이 아닌지
        /// </summary>
        public bool bIsDamaged => _iHP_Current < _iHP_MAX;

        /* protected & private - Field declaration         */

        [SerializeField]
        int _iHP_MAX;

        [SerializeField]
        int _iHP_Current;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 등록된 체력 이벤트를 초기화합니다
        /// </summary>
        public void DoClear_HealthEvent()
        {
            OnHealthEvent = null;
        }

        /// <summary>
        /// 체력을 강제로 Set합니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.None"/>입니다
        /// </summary>
        public void DoSet(int iHP)
        {
            _iHP_MAX = iHP;
            OnHealthEvent?.Invoke(EHealthEvent.None, _iHP_MAX, _iHP_Current, 0, GetHP_0_1());
        }

        /// <summary>
        /// 체력을 최대 체력만큼 회복합니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Reset"/>입니다
        /// </summary>
        public void DoReset()
        {
            _iHP_Current = _iHP_MAX;
            OnHealthEvent?.Invoke(EHealthEvent.Reset, _iHP_MAX, _iHP_Current, 0, 1);
        }

        /// <summary>
        /// 체력을 해당 양만큼 감소시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Damaged"/> 혹은  <see cref="EHealthEvent.Dead"/>입니다.
        /// </summary>
        /// <param name="iDamageAmount"></param>
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

        /// <summary>
        /// 체력을 회복시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Recovery"/>입니다
        /// </summary>
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