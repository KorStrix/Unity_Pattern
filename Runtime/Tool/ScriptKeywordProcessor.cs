// Tips from https://forum.unity3d.com/threads/c-script-template-how-to-make-custom-changes.273191/
using UnityEngine;
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
        string author = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        author = author.Contains("\\") ? author.Split('\\')[1] : author;
        // At this part you could actually get the name from Windows user directly or give it whatever you want
        fileContent = fileContent.Replace("#AUTHOR#", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1]);
        fileContent = fileContent.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("yyyy-MM-dd"));

        System.IO.File.WriteAllText(path, fileContent);
        AssetDatabase.Refresh();
    }
}