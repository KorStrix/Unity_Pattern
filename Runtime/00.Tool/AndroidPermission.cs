#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-03
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1

 *  참고한 링크 : 
 *  유니티 메뉴얼 - 안드로이드 퍼미션
 *  https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Android.Permission.RequestUserPermission.html
 *  
 *  안드로이드 메뉴얼 - 퍼미션 종류
 *  https://developer.android.com/reference/android/Manifest.permission.html?hl=ko
 *  
 *  https://mentum.tistory.com/150
   ============================================ */
#endregion Header

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace Unity_Pattern
{
    /// <summary>
    /// 매니페스트 선언필요.  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    /// </summary>
    public static class AndroidPermissionManager
    {
        /* const & readonly declaration             */

        const string const_strPermissionPrefix = "android.permission.";

        /* enum & struct declaration                */

        public enum EPermissionName
        {
            Camera,
            Microphone,
            FineLocation,
            CoarseLocation,

            // <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
            ExternalStorageRead,

            // <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
            ExternalStorageWrite,

            // <uses-permission android:name="android.permission.READ_PHONE_STATE" />
            Read_Phone_State
        }

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        static readonly Dictionary<EPermissionName, string> g_mapPermission = new Dictionary<EPermissionName, string>()
        {
            { EPermissionName.Camera, Permission.Camera },
            { EPermissionName.Microphone, Permission.Microphone },
            { EPermissionName.FineLocation, Permission.FineLocation },
            { EPermissionName.CoarseLocation, Permission.CoarseLocation },

            { EPermissionName.ExternalStorageRead, Permission.ExternalStorageRead },
            { EPermissionName.ExternalStorageWrite, Permission.ExternalStorageWrite },
            { EPermissionName.Read_Phone_State, const_strPermissionPrefix + "READ_PHONE_STATE" },
        };

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        static public bool Check_HasPermission(EPermissionName ePermissionName)
        {
            return Permission.HasUserAuthorizedPermission(g_mapPermission.GetValue_Safe(ePermissionName));
        }

        static public void RequestUserPermission_Coroutine(MonoBehaviour pCoroutineExecuter, EPermissionName ePermissionName, System.Action<bool> OnResult_HasPermission = null)
        {
#if UNITY_EDITOR
            OnResult_HasPermission?.Invoke(true);
#else
        pCoroutineExecuter.StartCoroutine(RequsetPermission_Coroutine(ePermissionName, OnResult_HasPermission));
#endif
        }


        // 해당 앱의 설정창을 호출한다.
        // https://forum.unity.com/threads/redirect-to-app-settings.461140/
        static public void DoOpenAppSetting()
        {
            try
            {
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                    {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        static IEnumerator RequsetPermission_Coroutine(EPermissionName ePermissionName, System.Action<bool> OnResult_HasPermission)
        {
            yield return new WaitForEndOfFrame();

            Permission.RequestUserPermission(g_mapPermission.GetValue_Safe(ePermissionName));

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => Application.isFocused == true);

            OnResult_HasPermission?.Invoke(Check_HasPermission(ePermissionName));
        }

        #endregion Private
    }
}