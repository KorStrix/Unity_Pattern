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
using System.Linq;

namespace Unity_Pattern
{
    /// <summary>
    /// 유닛의 체력을 담당하는 클래스
    /// </summary>
    public class Health : CObjectBase
    {
        /* const & readonly declaration             */

        readonly IDictionary<string, object> const_mapMsg_Empty = new Dictionary<string, object>();

        /* enum & struct declaration                */

        /// <summary>
        /// 체력 이벤트
        /// </summary>
        public enum EHealthEvent
        {
            None = 0,

            Reset = 1 << 0,
            Recovery = 1 << 1,
            Damaged = 1 << 2,
            Dead = 1 << 3,
        }

        /* public - Field declaration            */

        /// <summary>
        /// 체력 이벤트 메세지
        /// </summary>
        public struct OnHealthEventMsg
        {
            public Health pHealth;
            public IDictionary<string, object> mapMsg;

            public EHealthEvent eHealthEvent;
            public int iChangedHP_Origin;
            public int iChangedHP_Calculated;

            public OnHealthEventMsg(Health pHealth, IDictionary<string, object> mapMsg, EHealthEvent eHealthEvent, int iChangedHP_Origin, int iChangedHP_Calculated)
            {
                this.pHealth = pHealth; this.mapMsg = mapMsg;
                this.eHealthEvent = eHealthEvent; this.iChangedHP_Origin = iChangedHP_Origin; this.iChangedHP_Calculated = iChangedHP_Calculated;
            }
        }

        public delegate void delOnHealthEvent(OnHealthEventMsg sMsg);

        /// <summary>
        /// 체력 변동 이벤트가 생길때
        /// </summary>
        public event delOnHealthEvent OnHealthEvent;

        /// <summary>
        /// 현재 체력
        /// </summary>
        public int iHP => _iHP_Current;

        public int iHP_MAX => _iHP_MAX;

        public float fHP_0_1 => _iHP_Current / (float)_iHP_MAX;

        /// <summary>
        /// 현재 살아있는지
        /// </summary>
        public bool bIsAlive => _iHP_Current > 0;

        /// <summary>
        /// 체력이 최대체력이 아닌지
        /// </summary>
        public bool bIsDamaged => _iHP_Current < _iHP_MAX;

        /* protected & private - Field declaration         */

        Dictionary<EHealthEvent, IEnumerable<IHealthCalculateLogic>> _mapHealthLogic = new Dictionary<EHealthEvent, IEnumerable<IHealthCalculateLogic>>();

        [SerializeField]
        int _iHP_MAX;

        [SerializeField]
        int _iHP_Current;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInitLogic(HealthCalculateLogicFactory pLogicFactory)
        {
            _mapHealthLogic = pLogicFactory.arrLogicContainer.GroupBy(p => p.eEvent).
                                                              ToDictionary(p => p.Key,
                                                                           p => p.OrderBy(x => x.iOrder).Select(x => x.pLogic));
        }

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
            OnHealthEvent?.Invoke(new OnHealthEventMsg(this, const_mapMsg_Empty, EHealthEvent.None, 0, 0));
        }

        /// <summary>
        /// 체력을 최대 체력만큼 회복합니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Reset"/>입니다
        /// </summary>
        public void DoReset()
        {
            DoReset(null);
        }

        /// <summary>
        /// 체력을 최대 체력만큼 회복합니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Reset"/>입니다
        /// </summary>
        public void DoReset(IDictionary<string, object> mapMsg)
        {
            int iHPMax = _iHP_MAX;
            EHealthEvent eEvent = ExecuteLogic(EHealthEvent.Reset, ref iHPMax, ref mapMsg);
            _iHP_Current = iHPMax;

            OnHealthEvent?.Invoke(new OnHealthEventMsg(this, mapMsg, eEvent, 0, 0));
        }

        /// <summary>
        /// 체력을 해당 양만큼 감소시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Damaged"/> 혹은  <see cref="EHealthEvent.Dead"/>입니다.
        /// </summary>
        /// <param name="iDamageAmount"></param>
        public void DoDamage(int iDamageAmount)
        {
            DoDamage(iDamageAmount, null);
        }

        /// <summary>
        /// 체력을 해당 양만큼 감소시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Damaged"/> 혹은  <see cref="EHealthEvent.Dead"/>입니다.
        /// </summary>
        /// <param name="iDamageAmount"></param>
        public void DoDamage(int iDamageAmount, IDictionary<string, object> mapMsg)
        {
            if (_iHP_Current <= 0)
                return;

            int iOriginalValue = iDamageAmount;
            EHealthEvent eEvent = ExecuteLogic(EHealthEvent.Damaged, ref iDamageAmount, ref mapMsg);
            _iHP_Current -= iDamageAmount;
            if (_iHP_Current <= 0)
            {
                _iHP_Current = 0;
                eEvent = EHealthEvent.Dead;
            }

            OnHealthEvent?.Invoke(new OnHealthEventMsg(this, mapMsg, eEvent, iOriginalValue, iDamageAmount));
        }

        /// <summary>
        /// 바로 죽입니다.
        /// </summary>
        /// <param name="iDamageAmount"></param>
        public void DoKill()
        {
            DoKill(null);
        }

        /// <summary>
        /// 바로 죽입니다.
        /// </summary>
        /// <param name="iDamageAmount"></param>
        public void DoKill(IDictionary<string, object> mapMsg)
        {
            int iHPOrigin = _iHP_Current;
            _iHP_Current = 0;
            OnHealthEvent?.Invoke(new OnHealthEventMsg(this, mapMsg, EHealthEvent.Dead, iHPOrigin, iHPOrigin));
        }

        /// <summary>
        /// 체력을 회복시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Recovery"/>입니다
        /// </summary>
        public void DoRecovery(int iRecoveryAmount)
        {
            DoRecovery(iRecoveryAmount, null);
        }

        /// <summary>
        /// 체력을 회복시킵니다. <see cref="EHealthEvent"/>는 <see cref="EHealthEvent.Recovery"/>입니다
        /// </summary>
        public void DoRecovery(int iRecoveryAmount, IDictionary<string, object> mapMsg)
        {
            if (_iHP_Current == _iHP_MAX)
                return;

            int iOriginalValue = iRecoveryAmount;
            EHealthEvent eEvent = ExecuteLogic(EHealthEvent.Recovery, ref iRecoveryAmount, ref mapMsg);
            _iHP_Current += iRecoveryAmount;

            OnHealthEvent?.Invoke(new OnHealthEventMsg(this, mapMsg, eEvent, iOriginalValue, iRecoveryAmount));
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            if(_mapHealthLogic.Count == 0)
            {
                HealthCalculateLogicFactory pFactory = new HealthCalculateLogicFactory();
                pFactory.DoCreate_LibraryLogic(EHealthCalculateLogicName.LimitHP, EHealthEvent.Recovery);

                DoInitLogic(pFactory);
            }
        }

        protected override void OnEnableObject()
        {
            base.OnEnableObject();

            DoReset();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private EHealthEvent ExecuteLogic(EHealthEvent eEvent, ref int iHPMax, ref IDictionary<string, object> mapMsg)
        {
            if (mapMsg == null)
                mapMsg = const_mapMsg_Empty;

            IEnumerable<IHealthCalculateLogic> arrLogic;
            if (_mapHealthLogic.TryGetValue(eEvent, out arrLogic))
            {
                foreach (var pLogic in arrLogic)
                    pLogic.CalculateHealth(this, const_mapMsg_Empty, ref eEvent, ref iHPMax);
            }

            return eEvent;
        }

        #endregion Private
    }
}