#region Header
/*	============================================
 *	Author   			    : Strix
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
	/// <see cref="IBuffAttribute"/>를 담는 Container.
	/// 
	/// <para>버프는 버프효과 리스트를 담고있습니다.</para>
	/// </summary>
	public interface IBuffContainer
	{
		/// <summary>
		/// 버프 고유 키
		/// </summary>
		string IBuffContainer_strBuffKey { get; }

		/// <summary>
		/// 버프 지속시간
		/// </summary>
		float IBuffContainer_fDurationSec { get; }

		IEnumerable<IBuffAttribute> IBuffContainer_arrAttribute { get; }

		void IBuffContainer_OnReset();
		void IBuffContainer_OnPlay();
	}

	/// <summary>
	/// 버프 효과
	/// </summary>
	public interface IBuffAttribute
	{
		/// <summary>
		/// 버프 효과의 고유 키
		/// </summary>
		string IBuffAttribute_strBuffAttributeKey { get; }


		void IBuffAttribute_OnReset();
		void IBuffAttribute_OnPlay();
	}

	/// <summary>
	/// 
	/// </summary>
	public class BuffContainerComponent : CObjectBase
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		[System.Serializable]
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
				this.fRemainSec = bIsEternal ? 999f : pBuffContainer.IBuffContainer_fDurationSec;

				pBuffContainer.IBuffContainer_OnReset();
				pBuffContainer.IBuffContainer_arrAttribute.ForEachCustom(p => p.IBuffAttribute_OnReset());
			}

			public void DoPlay()
			{
				pBuffContainer.IBuffContainer_OnPlay();
				pBuffContainer.IBuffContainer_arrAttribute.ForEachCustom(p => p.IBuffAttribute_OnPlay());
			}

			public void DoSet_IsEternal(bool bIsEternal)
			{
				this.bIsEternal = bIsEternal;
			}

			public void DoUpdate(float fDeltaTime)
			{
				if (bIsEternal)
					return;

				fRemainSec -= fDeltaTime;
			}
		}

		/* public - Field declaration               */


		/* protected & private - Field declaration  */

		Dictionary<IBuffContainer, BuffContainerWrapper> _mapBuffContainerWrapper = new Dictionary<IBuffContainer, BuffContainerWrapper>();

		List<BuffContainerWrapper> _listCurrentBuff = new List<BuffContainerWrapper>();

		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

		public void DoInit(IEnumerable<IBuffContainer> listBuffContainer, bool bIsPlay)
		{
			_mapBuffContainerWrapper.Clear();
			foreach (var pBuffContainer in listBuffContainer)
				_mapBuffContainerWrapper.Add_Safe(pBuffContainer, new BuffContainerWrapper(pBuffContainer));

			if (bIsPlay)
				DoPlayBuff();
		}

		public void DoPlayBuff(bool bIsReset = true)
		{
			_listCurrentBuff.Clear();
			_listCurrentBuff.AddRange(_mapBuffContainerWrapper.Values);

			if(bIsReset)
				_listCurrentBuff.ForEachCustom(p => p.DoReset());

			_listCurrentBuff.ForEachCustom(p => p.DoPlay());
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