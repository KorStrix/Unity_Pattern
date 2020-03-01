#region Header
/*	============================================
 *	Aurthor 			  : Strix
 *	Initial Creation Date : 2020-02-17
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity_Pattern.HealthCalculateLogic;

namespace Unity_Pattern
{
    public interface IHealthCalculateLogic
    {
        void CalculateHealth(Health pHealth, IDictionary<string, object> mapMsg, ref Health.EHealthEvent eEvent, ref int iAmount);
    }

    public enum EHealthCalculateLogicName
    {
        LimitHP,
    }

    public class HealthCalculateLogicFactory
    {
        public IEnumerable<HealthCalculateLogic_Container> arrLogicContainer => _listLogicContainer;

        List<HealthCalculateLogic_Container> _listLogicContainer = new List<HealthCalculateLogic_Container>();

        public IHealthCalculateLogic DoCreate_LibraryLogic(EHealthCalculateLogicName eLogic, Health.EHealthEvent eEvent, int iOrder = 0)
        {
            IHealthCalculateLogic pLogic = null;
            switch (eLogic)
            {
                case EHealthCalculateLogicName.LimitHP: pLogic = new Limit_MaxHP(); break;


                default: Debug.LogError("Error - Not Found Logic"); return null;
            }

            _listLogicContainer.Add(new HealthCalculateLogic_Container(eEvent, iOrder, pLogic));
            return pLogic;
        }

        public void DoAdd_CustomLogic(IHealthCalculateLogic pLogic, Health.EHealthEvent eEvent, int iOrder = 0)
        {
            _listLogicContainer.Add(new HealthCalculateLogic_Container(eEvent, iOrder, pLogic));
        }
    }


    namespace HealthCalculateLogic
    {
        public class Limit_MaxHP : IHealthCalculateLogic
        {
            public void CalculateHealth(Health pHealth, IDictionary<string, object> mapMsg, ref Health.EHealthEvent eEvent, ref int iAmount)
            {
                if (pHealth.iHP + iAmount > pHealth.iHP_MAX)
                    iAmount = pHealth.iHP_MAX - pHealth.iHP;
            }
        }

        public struct HealthCalculateLogic_Container
        {
            public Health.EHealthEvent eEvent;
            public int iOrder;

            public IHealthCalculateLogic pLogic;

            public HealthCalculateLogic_Container(Health.EHealthEvent eEvent, int iOrder, IHealthCalculateLogic pLogic)
            {
                this.eEvent = eEvent; this.iOrder = iOrder; this.pLogic = pLogic;
            }
        }
    }

}