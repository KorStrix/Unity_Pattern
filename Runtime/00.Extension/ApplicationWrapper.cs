#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-16 오전 11:10:06
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// https://answers.unity.com/questions/161858/startstop-playmode-from-editor-script.html
/// </summary>
public static class ApplicationWrapper
{
    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void VibrateDevice()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    //public static void VibrateDevice(long milliseconds)
    //{
    //    Vibrator.Call("vibrate", milliseconds);
    //}

    //public static void VibrateDevice(long[] pattern, int repeat)
    //{
    //    Vibrator.Call("vibrate", pattern, repeat);
    //}

    //// https://www.reddit.com/r/Unity3D/comments/4j5js7/unity_vibrate_android_device_for_custom_duration/
    //private static readonly AndroidJavaObject Vibrator =
    //         new AndroidJavaClass("com.unity3d.player.UnityPlayer")// Get the Unity Player.
    //        .GetStatic<AndroidJavaObject>("currentActivity")// Get the Current Activity from the Unity Player.
    //        .Call<AndroidJavaObject>("getSystemService", "vibrator");// Then get the Vibration Service from the Current Activity.
}