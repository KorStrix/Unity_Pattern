// Tips from https://forum.unity3d.com/threads/c-script-template-how-to-make-custom-changes.273191/
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

internal sealed class ScriptKeywordProcessor : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string strPath)
    {
        if (strPath.Contains(nameof(ScriptKeywordProcessor)))
            return;

        strPath = strPath.Replace(".meta", "");
        int iIndex = strPath.LastIndexOf(".");
        if (iIndex < 0)
            return;

        string strFile = strPath.Substring(iIndex);
        if (strFile != ".cs")
            return;

        iIndex = Application.dataPath.LastIndexOf("Assets");
        strPath = Application.dataPath.Substring(0, iIndex) + strPath;
        if (System.IO.File.Exists(strPath) == false)
            return;

        string strFileContent = System.IO.File.ReadAllText(strPath);
        strFileContent = strFileContent.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("yyyy-MM-dd"));
        strFileContent = Replace_Author(strFileContent);

        System.IO.File.WriteAllText(strPath, strFileContent);
        AssetDatabase.Refresh();
    }

    private static string Replace_Author(string strFileContent)
    {
        var listSaveData = PlayerPrefWindowEditor.GetPlayerPrefSaveDataList();
        var pAurthorData = listSaveData.Where(p => p.strKey.ToLower().Contains("author")).
                                        Where(p => p.eFieldType == PlayerPrefWindowEditor.EFieldType.String).
                                        FirstOrDefault();

        if (pAurthorData == null)
            Debug.LogError("PlayerPref - AUTHOR(string) is Null Or Empty!!");

        strFileContent = strFileContent.Replace("#AUTHOR#", pAurthorData.strValue);
        return strFileContent;
    }
}
#endif