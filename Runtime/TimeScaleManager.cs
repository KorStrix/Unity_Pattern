#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-09 오후 12:04:30
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
    public class TimeScaleManager : CSingletonDynamicMonoBase<TimeScaleManager>
    {
        /* const & readonly declaration             */

        const bool const_bIsDebug = false;

        /* enum & struct declaration                */

        public enum EOnOverFlow
        {
            Current_To_Min,
        }

        public struct OnChangeTimeScaleMsg
        {
            public float fTimeScale_Prev;
            public float fTimeScale_Current;

            public OnChangeTimeScaleMsg(float fTimeScale_Prev, float fTimeScale_Current)
            {
                this.fTimeScale_Prev = fTimeScale_Prev; this.fTimeScale_Current = fTimeScale_Current;
            }
        }

        /* public - Field declaration            */

        public ObservableCollection<OnChangeTimeScaleMsg> OnChangeTimeScale { get; private set; } = new ObservableCollection<OnChangeTimeScaleMsg>();

        public float fTimeScale_Current 
        {
            get => Time.timeScale;
            private set => Time.timeScale = value;
        }

        public float fTimeScale_Default = 1f;
        public float fTimeScale_Min = 1f;
        public float fTimeScale_Max = 3f;


        /* protected & private - Field declaration         */

        float _fTimeScale_Prev;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoAddTimeScale(float fTimeScaleAdd, EOnOverFlow eOnOverFlow = EOnOverFlow.Current_To_Min)
        {
            float fTimeScale = fTimeScale_Current + fTimeScaleAdd;
            if(fTimeScale > fTimeScale_Max)
            {
                switch (eOnOverFlow)
                {
                    case EOnOverFlow.Current_To_Min:
                        fTimeScale = fTimeScale_Min;
                        break;
                }
            }


            SetTimeScale(fTimeScale);
        }

        public void DoSetTimeScale(float fTimeScale)
        {
            SetTimeScale(fTimeScale);
        }

        public void DoSetTimeScale_Prev()
        {
            SetTimeScale(_fTimeScale_Prev);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTimeScale(fTimeScale_Default);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void SetTimeScale(float fTimeScale)
        {
            if(const_bIsDebug)
                Debug.Log($"{name} -{nameof(SetTimeScale)} : {fTimeScale}");

            _fTimeScale_Prev = fTimeScale_Current;
            fTimeScale_Current = fTimeScale;
            OnChangeTimeScale.DoNotify(new OnChangeTimeScaleMsg(_fTimeScale_Prev, fTimeScale_Current));
        }

        #endregion Private
    }
}