#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-03-15
 *	Summary 		        : 
 *	
 *	원본 코드 : https://forum.unity.com/threads/editor-utility-player-prefs-editor-edit-player-prefs-inside-the-unity-editor.370292/
 *	
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Microsoft.Win32;

/// <summary>
/// 
/// </summary>
public class PlayerPrefWindowEditor : EditorWindow
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class PlayerPrefSaveData
    {
        public string strKey { get; private set; }
        public EFieldType eFieldType { get; private set; }

        public int iValue { get; private set; }
        public float fValue { get; private set; }
        public string strValue { get; private set; }

        public object pValue_Origin { get; private set; }

        public PlayerPrefSaveData(string strKey, int iValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.Integer;
            this.iValue = iValue;
        }

        public PlayerPrefSaveData(string strKey, float fValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.Float;
            this.fValue = fValue;
        }

        public PlayerPrefSaveData(string strKey, string strValue, object pValue_Origin)
        {
            this.strKey = strKey; this.pValue_Origin = pValue_Origin;

            eFieldType = EFieldType.String;
            this.strValue = strValue.Replace("\0", "");
        }
    }

    public enum EFieldType { String, Integer, Float }

    /* public - Field declaration               */


    /* protected & private - Field declaration  */

    private EFieldType _eFieldType = EFieldType.String;

    private string _strKey = "";
    private string _strSetValue = "";
    private string _strGetValue = "";

    private string _strError = null;
    private string _strLog = null;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    public static List<PlayerPrefSaveData> GetPlayerPrefSaveDataList()
    {
        var listResult = new List<PlayerPrefSaveData>();

        using (var pHiveKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
        {
            using (var pCurrentKey = pHiveKey.OpenSubKey(GetRegistryPath()))
            {
                string[] arrValueNames = pCurrentKey.GetValueNames();
                for (int i = 0; i < arrValueNames.Length; i++)
                {
                    string strValueName = arrValueNames[i];
                    object pValue = pCurrentKey.GetValue(strValueName);
                    string strTypeName = pValue.GetType().Name.ToLower();

                    if (pValue is int)
                        listResult.Add(new PlayerPrefSaveData(strValueName, (int)pValue, pValue));

                    else if (pValue is float)
                        listResult.Add(new PlayerPrefSaveData(strValueName, (float)pValue, pValue));

                    else if (pValue is byte[])
                        listResult.Add(new PlayerPrefSaveData(strValueName, System.Text.Encoding.Default.GetString(pValue as byte[]), pValue));
                }
            }
        }

        return listResult;
    }

    static string GetRegistryPath()
    {
        return $"Software\\Unity\\UnityEditor\\{PlayerSettings.companyName}\\{PlayerSettings.productName}";
    }


    [MenuItem("Tools/PlayerPref Editor")]
    static void Init()
    {
        PlayerPrefWindowEditor pWindow = (PlayerPrefWindowEditor)GetWindow(typeof(PlayerPrefWindowEditor), false);

        pWindow.minSize = new Vector2(600, 300);
        pWindow.Show();
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Player Prefs Editor", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("by RomejanicDev");
        EditorGUILayout.Separator();

        _eFieldType = (EFieldType)EditorGUILayout.EnumPopup("Key Type", _eFieldType);
        _strKey = EditorGUILayout.TextField("Pref Key", _strKey);
        _strSetValue = EditorGUILayout.TextField("Pref Save Value", _strSetValue);
        EditorGUILayout.LabelField("Pref Get Value", _strGetValue);

        if (string.IsNullOrEmpty(_strError) == false)
        {
            EditorGUILayout.HelpBox(_strError, MessageType.Error);
        }

        if (string.IsNullOrEmpty(_strLog) == false)
        {
            EditorGUILayout.HelpBox(_strLog, MessageType.None);
        }

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Set Key"))
            {
                if (_eFieldType == EFieldType.Integer)
                {
                    int iResult;
                    if (!int.TryParse(_strSetValue, out iResult))
                    {
                        _strError = "Invalid input \"" + _strSetValue + "\"";
                        return;
                    }
                    PlayerPrefs.SetInt(_strKey, iResult);
                    _strLog = $"Set {_strKey} - {iResult}";
                }
                else if (_eFieldType == EFieldType.Float)
                {
                    float fResult;
                    if (!float.TryParse(_strSetValue, out fResult))
                    {

                        _strError = "Invalid input \"" + _strSetValue + "\"";
                        return;
                    }
                    PlayerPrefs.SetFloat(_strKey, fResult);
                    _strLog = $"Set {_strKey} - {fResult}";
                }
                else
                {
                    PlayerPrefs.SetString(_strKey, _strSetValue);
                    _strLog = $"Set {_strKey} - {_strSetValue}";
                }

                PlayerPrefs.Save();
                _strError = null;
            }

            if (GUILayout.Button("Get Key"))
            {
                if (_eFieldType == EFieldType.Integer)
                {
                    _strGetValue = PlayerPrefs.GetInt(_strKey).ToString();
                }
                else if (_eFieldType == EFieldType.Float)
                {
                    _strGetValue = PlayerPrefs.GetFloat(_strKey).ToString();
                }
                else
                {
                    _strGetValue = PlayerPrefs.GetString(_strKey);
                }

                _strLog = $"Get {_strKey} - {_strGetValue}";
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Delete Key"))
            {
                PlayerPrefs.DeleteKey(_strKey);
                PlayerPrefs.Save();
            }

            if (GUILayout.Button("Delete All Keys"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
        GUILayout.EndHorizontal();
    }


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}