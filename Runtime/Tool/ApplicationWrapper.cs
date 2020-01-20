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
}