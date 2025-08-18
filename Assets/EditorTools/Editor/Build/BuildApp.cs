using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;


public class BuildApp : EditorWindow
{
    [MenuItem("Build/打包APP")]
    public static void ShowWin()
    {
        var win = GetWindow<BuildApp>();
        win.Show();
    }

    private void Awake()
    {
        m_versionGUI = new VersionGUI();
        m_versionGUI.Awake();
    }

    private void OnGUI()
    {
        if (m_versionGUI == null)
        {
            m_versionGUI = new VersionGUI();
            m_versionGUI.Awake();
        }
        m_versionGUI.DrawVersion();
        DrawBuildApp();
    }

    

    private void DrawBuildApp()
    {
        if(GUILayout.Button("Build APP"))
        {
            // 生成原始lua全量文件的md5
            BuildUtils.GenOriginalLuaFrameworkMD5File();
            // 打AssetBundle
            BuildAssetBundle.Build();
            // 打包APP
            BuildUtils.BuildApp();
        }
    }

    private VersionGUI m_versionGUI;
}
