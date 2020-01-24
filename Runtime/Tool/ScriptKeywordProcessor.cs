// Tips from https://forum.unity3d.com/threads/c-script-template-how-to-make-custom-changes.273191/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

internal sealed class ScriptKeywordProcessor : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        if (index < 0)
            return;

        string file = path.Substring(index);
        if (file != ".cs" && file != ".js")
            return;

        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        if (!System.IO.File.Exists(path))
            return;

        string fileContent = System.IO.File.ReadAllText(path);
        // At this part you could actually get the name from Windows user directly or give it whatever you want
        fileContent = fileContent.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("yyyy-MM-dd"));

        System.IO.File.WriteAllText(path, fileContent);
        AssetDatabase.Refresh();
    }
}
#endif