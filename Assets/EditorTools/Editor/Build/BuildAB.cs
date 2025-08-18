using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;

public class BuildAB : EditorWindow
{
    [MenuItem("Build/打包AB")]
    public static void ShowWin_AB()
    {
        var win = GetWindow<BuildAB>();
        win.Show();
    }

    private HotfixConfigInfo hotfixConfigInfo;
    private string configPath;
    private void Awake()
    {
        hotfixConfigInfo = InitHotfixPath();
        m_versionGUI = new VersionGUI();
        m_versionGUI.Awake();
    }

    HotfixConfigInfo InitHotfixPath()
    {
        configPath = Application.dataPath + "/Resources/hotfixPathConfig.txt";
        if (!File.Exists(configPath))
        {
            FileStream stream = File.Create(configPath);
            stream.Close();
        }
        string jsonStr = File.ReadAllText(configPath);
        HotfixConfigInfo jsonData = new HotfixConfigInfo();
        if (!string.IsNullOrEmpty(jsonStr))
        {
            jsonData = JsonMapper.ToObject<HotfixConfigInfo>(jsonStr);
        }
        return jsonData;
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

    private string cfg_Path;
    private string prefab_Path;

    private void DrawBuildApp()
    {
        if (hotfixConfigInfo == null)
        {
            hotfixConfigInfo = InitHotfixPath();
        }

        GUILayout.BeginHorizontal();
        hotfixConfigInfo.cfg_Path = EditorGUILayout.TextField("cfg_Path(相对路径)", hotfixConfigInfo.cfg_Path);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        hotfixConfigInfo.prefab_Path = EditorGUILayout.TextField("prefab_Path(相对路径)", hotfixConfigInfo.prefab_Path);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        hotfixConfigInfo.luaFile_Path = EditorGUILayout.TextField("luaFile_Path(相对路径)", hotfixConfigInfo.luaFile_Path);
        hotfixConfigInfo.toluaFile_Path = EditorGUILayout.TextField("toluaFile_Path(相对路径)", hotfixConfigInfo.toluaFile_Path);
        hotfixConfigInfo.isAESEncrypt=EditorGUILayout.Toggle("Lua是否加密(默认是加密) ",hotfixConfigInfo.isAESEncrypt);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        hotfixConfigInfo.audio_Path = EditorGUILayout.TextField("audio_Path(相对路径)", hotfixConfigInfo.audio_Path);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SavePath"))
        {
            //写入配置
            JsonWriter writer = new JsonWriter();
            writer.PrettyPrint = true;  //写入json时自动换行
            JsonMapper.ToJson(hotfixConfigInfo, writer);
            File.WriteAllText(configPath, writer.TextWriter.ToString());
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();


        if (GUILayout.Button("生成MD5文件"))
        {
            // 打AssetBundle
            BuildAssetBundle.BuildABMD5List(hotfixConfigInfo);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("生成压缩包"))
        {
            //string zipDir = Application.streamingAssetsPath + "/HotfixRootPath";
            //BuildAssetBundle.ZipUpdateDir(zipDir, VersionMgr.instance.resVersion);
            //Debug.Log("生成压缩包成功");
            //AssetDatabase.Refresh();
        }


        if (GUILayout.Button("Build APP"))
        {
            //// 生成原始lua全量文件的md5
            //BuildUtils.GenOriginalLuaFrameworkMD5File();

            //// 打AssetBundle
            ////BuildAssetBundle.Build();
            //BuildAssetBundle.BuildABMD5List(hotfixConfigInfo);

            //// 打包APP
            //BuildUtils.BuildApp();
        }
    }

    private VersionGUI m_versionGUI;

    public class HotfixConfigInfo
    {
        public string cfg_Path="";
        public string prefab_Path="";
        public string luaFile_Path="";
        public string toluaFile_Path="";
        public string audio_Path="";
        /// <summary>
        /// 是否加密
        /// </summary>
        public bool isAESEncrypt = true;
    }
}

