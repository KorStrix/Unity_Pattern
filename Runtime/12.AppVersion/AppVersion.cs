#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-03-20
 *	Summary 		        : 
 *	
 *	사용되는 Define
 *	
 *	유니티
 *	UNITY_EDITOR, UNITY_ANDROID, UNITY_IOS
 *	
 *	
 *	빌드 페이즈 (커스텀)
 *	ALPHA, BETA, LIVE
 *	
 *	
 *	마켓 (커스텀)
 *	PLAYSTORE, ONESTORE, GALAXYSTORE, APPSTORE
 *	
 *	
 *  Template 		        : For Unity Editor V1
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
	public class AppVersion
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/// <summary>
		/// 
		/// </summary>
		public enum EBuildPhase
		{
			/// <summary>
			/// 개발실 전용 빌드
			/// </summary>
			[SubString("d")]
			Dev,

			/// <summary>
			/// QA 및 사내 내부 테스트용 빌드
			/// </summary>
			[SubString("a")]
			Alpha,

			/// <summary>
			/// 클로즈/오픈 베타용 빌드
			/// </summary>
			[SubString("b")]
			Beta,

			/// <summary>
			/// 서비스용 빌드
			/// </summary>
			[SubString("l")]
			Live,
		}

		/// <summary>
		/// 서비스 할 플랫폼
		/// </summary>
		[System.Flags]
		public enum EPlatformTypeFlag
		{
			[SubString("")]
			None = 0,

			[SubString("e")]
			Editor = 1 << 1,
			[SubString("p")]
			PC = 1 << 2,
			[SubString("a")]
			Android = 1 << 3,
			[SubString("i")]
			IOS = 1 << 4,
		}

		[System.Flags]
		public enum EMarketTypeFlag
		{
			[SubString("")]
			None = 0,

			[SubString("p")]
			PlayStore = 1 << 1,
			[SubString("o")]
			OneStore = 1 << 2,
			[SubString("g")]
			GalaxyStore = 1 << 3,

			[SubString("a")]
			AndroidStore = PlayStore | OneStore | GalaxyStore,

			[SubString("i")]
			AppStore = 1 << 4,
		}

		/* public - Field declaration               */

		public static string strVersion;

		/// <summary>
		/// 현재 빌드 페이즈(<see cref="EBuildPhase.Alpha"/> or <see cref="EBuildPhase.Beta"/> 등)
		/// </summary>
		public static EBuildPhase eBuildPhase
		{
			get
			{
#if ALPHA
				return EBuildPhase.Alpha;
#elif BETA
				return EBuildPhase.Beta;
#elif LIVE
				return EBuildPhase.Live;
#endif
				return EBuildPhase.Dev;
			}
		}

		/// <summary>
		/// 현재 서비스 플랫폼
		/// </summary>
		public static EPlatformTypeFlag ePlatformTypeFlag
		{
			get
			{
				EPlatformTypeFlag eFlag = EPlatformTypeFlag.None;

#if UNITY_EDITOR
				eFlag |= EPlatformTypeFlag.Editor;
#endif

#if UNITY_STANDALONE
				eFlag |= EPlatformTypeFlag.PC;
#elif UNITY_ANDROID
				eFlag |= EPlatformTypeFlag.Android;
#elif UNITY_IOS
				eFlag |= EPlatformTypeFlag.IOS;
#endif

				return eFlag;
			}
		}

		/// <summary>
		/// 현재 서비스 마켓
		/// </summary>
		public static EMarketTypeFlag eMarketTypeFlag
		{
			get
			{
				EMarketTypeFlag eFlag = EMarketTypeFlag.None;

#if PLAYSTORE
				eFlag |= EMarketTypeFlag.PlayStore;
#endif
#if ONESTORE
				eFlag |= EMarketTypeFlag.OneStore;
#endif
#if GALAXYSTORE
				eFlag |= EMarketTypeFlag.GalaxyStore;
#endif
#if APPSTORE
				eFlag |= EMarketTypeFlag.AppStore;
#endif

				return eFlag;
			}
		}

		/* protected & private - Field declaration  */


		// ========================================================================== //

		/* public - [Do~Something] Function 	        */


		// ========================================================================== //

		/* protected - [Override & Unity API]       */


		/* protected - [abstract & virtual]         */


		// ========================================================================== //

#region Private

#endregion Private
	}
}