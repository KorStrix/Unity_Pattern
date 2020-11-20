#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-06-22
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class CEventTrigger : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [System.Flags]
        public enum EEventTypeFlag
        {
            OnAwake = 1 << 0,
            OnStart = 1 << 1,
            OnEnable = 1 << 2,
            OnDisable = 1 << 3,
            OnDestroy = 1 << 4,
        }

        [System.Serializable]
        public class EventInfo : UnityEvent<CEventTrigger>
        {
        }

        [System.Serializable]
        public class TriggerInfo
        {
            [Header("플래그라 동시에 여러이벤트 가능")]
            [EnumFlag]
            public EEventTypeFlag eEventTypeFlag;

            public float fDelaySec;
            public EventInfo OnEvent;

            public void DoInvokeEvent(CEventTrigger pTrigger)
            {
                if (fDelaySec <= 0.02f)
                    OnEvent?.Invoke(pTrigger);
                else
                    pTrigger.StartCoroutine(DelayInvoke(pTrigger, fDelaySec));
            }

            IEnumerator DelayInvoke(CEventTrigger pTrigger, float fDelay)
            {
                yield return new WaitForSeconds(fDelay);

                OnEvent?.Invoke(pTrigger);
            }
        }

        /* public - Field declaration               */

        public List<TriggerInfo> listTriggerInfo = new List<TriggerInfo>();

        /* protected & private - Field declaration  */

        private Dictionary<EEventTypeFlag, List<TriggerInfo>> _mapTriggerInfo = new Dictionary<EEventTypeFlag, List<TriggerInfo>>();

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _mapTriggerInfo.GetValue_OrDefault(EEventTypeFlag.OnAwake)?.ForEachCustom(p => p.DoInvokeEvent(this));
        }

        private void Start()
        {
            _mapTriggerInfo.GetValue_OrDefault(EEventTypeFlag.OnStart)?.ForEachCustom(p => p.DoInvokeEvent(this));
        }

        protected override void OnEnableObject()
        {
            base.OnEnableObject();

            Init();

            _mapTriggerInfo.GetValue_OrDefault(EEventTypeFlag.OnEnable)?.ForEachCustom(p => p.DoInvokeEvent(this));
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            if (bIsQuit_Application == false)
                _mapTriggerInfo.GetValue_OrDefault(EEventTypeFlag.OnDisable)?.ForEachCustom(p => p.DoInvokeEvent(this));
        }

        protected override void OnDestroyObject(bool bIsQuit_Application)
        {
            base.OnDestroyObject(bIsQuit_Application);

            if(bIsQuit_Application == false)
                _mapTriggerInfo.GetValue_OrDefault(EEventTypeFlag.OnDestroy)?.ForEachCustom(p => p.DoInvokeEvent(this));
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void Init()
        {
            _mapTriggerInfo.Clear();

            string[] arrFlags = System.Enum.GetNames(typeof(EEventTypeFlag));
            for (int i = 0; i < arrFlags.Length; i++)
            {
                EEventTypeFlag eFlag = (EEventTypeFlag)System.Enum.Parse(typeof(EEventTypeFlag), arrFlags[i]);
                _mapTriggerInfo[eFlag] = new List<TriggerInfo>(listTriggerInfo.Where(p => p.eEventTypeFlag.HasFlag_Custom(eFlag)));
            }
        }

        #endregion Private
    }
}