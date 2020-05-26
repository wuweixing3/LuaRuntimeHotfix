using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using XLua;

[CSharpCallLua]
public delegate void ReloadDelegate(string path);

public  class LuaFileWatcher
{
    
    private const string PrePath = "LuaScripts";
    
    private static ReloadDelegate _reloadFunction;
    
    private static HashSet<string> _changedFiles = new HashSet<string>();
    
    public static void CreateLuaFileWatcher(LuaEnv luaEnv)
    {
        var scriptPath = Path.Combine(Application.dataPath, PrePath);
        DirectoryWatcher.CreateWatch(scriptPath, new FileSystemEventHandler(OnLuaFileChanged));
        _reloadFunction = luaEnv.Global.Get<ReloadDelegate>("hotfix");
        EditorApplication.update -= Reload;
        EditorApplication.update += Reload;
    }

    private static void OnLuaFileChanged(object obj, FileSystemEventArgs args)
    {
        var fullPath = args.FullPath;
        var luaFolderName = PrePath;
        var requirePath = fullPath.Replace(".lua", "");
        var luaScriptIndex = requirePath.IndexOf(luaFolderName, StringComparison.Ordinal) + luaFolderName.Length + 1;
        requirePath = requirePath.Substring(luaScriptIndex);
        requirePath = requirePath.Replace('\\','.');
        _changedFiles.Add(requirePath);
    }

    private static void Reload()
    {
        if (EditorApplication.isPlaying == false)
        {
            return;
        }
        if (_changedFiles.Count == 0)
        {
            return;
        }

        foreach (var file in _changedFiles)
        {
            _reloadFunction(file);
            Debug.Log("Reload:" + file);
        }
        _changedFiles.Clear();
    }
}