#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-03-20
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
	/// <summary>
	/// <see cref="IBuffAttribute"/>�� ��� Container
	/// </summary>
	public interface IBuffContainer
	{
		/// <summary>
		/// ���� ���� Ű
		/// </summary>
		string IBuffContainer_strBuffKey { get; }

		/// <summary>
		/// ���� ���ӽð�
		/// </summary>
		float IBuffContainer_fDurationSec { get; }

		IEnumerable<IBuffAttribute> IBuffContainer_arrAttribute { get; }
	}

	public interface IBuffAttribute
	{
		/// <summary>
		/// ���� ���� Ű
		/// </summary>
		string IBuffAttribute_strBuffAttributeKey { get; }

		/// <summary>
		/// ���� ���ӽð�
		/// </summary>
		float IBuffData_fDurationSec { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BuffContainerComponent : CObjectBase
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		public class BuffContainerWrapper
		{
			public IBuffContainer pBuffContainer { get; private set; }
			public float fRemainSec { get; private set; }

			public bool bIsEternal { get; private set; }
			public BuffContainerWrapper(IBuffContainer pBuffContainer)
			{
				this.pBuffContainer = pBuffContainer;
				this.bIsEternal = false;

				DoReset();
			}

			public void DoReset()
			{
				this.fRemainSec = pBuffContainer.IBuffContainer_fDurationSec;
			}

			public void DoUpdate(float fDeltaTime)
			{
				fRemainSec -= fDeltaTime;
			}
		}

		/* public - Field declaration               */


		/* protected & private - Field declaration  */

		Dictionary<IBuffContainer, BuffContainerWrapper> _mapBuffContainerWrapper = new Dictionary<IBuffContainer, BuffContainerWrapper>();

		// ========================================================================== //

		/* public - [Do~Somthing] Function 	        */

		public void DoInit(IEnumerable<IBuffContainer> listBuffContainer)
		{
			_mapBuffContainerWrapper.Clear();
			foreach (var pBuffContainer in listBuffContainer)
				_mapBuffContainerWrapper.Add_Safe(pBuffContainer, new BuffContainerWrapper(pBuffContainer));
		}

		// ========================================================================== //

		/* protected - [Override & Unity API]       */

		private void Update()
		{
			float fTime = Time.deltaTime;
			foreach(var pBuffContainer in _mapBuffContainerWrapper.Values)
			{
				pBuffContainer.DoUpdate(fTime);

				if(pBuffContainer.fRemainSec <= 0f)
				{

				}
			}

		}

		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		#endregion Private
	}
}