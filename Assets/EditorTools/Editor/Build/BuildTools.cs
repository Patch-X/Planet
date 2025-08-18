using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor.Build.Player;
using YooAsset.Editor;
using YooAsset;
using System;
using System.Linq;

public class BuildTools : EditorWindow
{
    [MenuItem("BuildTools/移动HotfixUpdate.dll到GameRes文件夹")]
    public static void ShowWin_MoveDll()
    {
        var win = GetWindow<BuildTools>();
        win.Show();
    }
    
    [MenuItem("BuildTools/BuildAPKPipeline")]
    public static void BuildAPKPipeline()
    {
        BuildTarget _buildTarget = EditorUserBuildSettings.activeBuildTarget;
        CompileDllCommand.CompileDll(_buildTarget);
        Debug.Log("*** compile finish!!!  ****");


        MoveHotfixUpdateDll();
        Debug.Log("*** 移动HotfixUpdate完成  ****");
        //AssetBundleBuilderWindow window = GetWindow<AssetBundleBuilderWindow>("资源包构建工具", true, WindowsDefine.DockedWindowTypes);
        //window.ExecuteBuild();
        ExecuteBuild();
        Debug.Log("*** 资源包构建完成  ****");
        
        BuildPlayerOptions options = new BuildPlayerOptions();
        string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenePaths[i] = EditorBuildSettings.scenes[i].path;
        }
        options.scenes = scenePaths;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;
        //projectPath  =>  E:/MyFramework/my_framework/
        string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        options.locationPathName = $"{projectPath}BuildApk/Apk.apk";
        BuildPipeline.BuildPlayer(options);
        Debug.Log("*** 打Apk完成  ****");
    }

    private static void ExecuteBuild()
    {
        BuildParameters buildParameters = new BuildParameters();
        buildParameters.StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot();
        buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        buildParameters.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        buildParameters.BuildPipeline = AssetBundleBuilderSettingData.Setting.BuildPipeline;
        buildParameters.BuildMode = AssetBundleBuilderSettingData.Setting.BuildMode;
        buildParameters.PackageName = AssetBundleBuilderSettingData.Setting.BuildPackage;
        buildParameters.PackageVersion = GetBuildPackageVersion();
        buildParameters.VerifyBuildingResult = true;
        buildParameters.SharedPackRule = new ZeroRedundancySharedPackRule();
        buildParameters.EncryptionServices = CreateEncryptionServicesInstance();
        buildParameters.CompressOption = AssetBundleBuilderSettingData.Setting.CompressOption;
        buildParameters.OutputNameStyle = AssetBundleBuilderSettingData.Setting.OutputNameStyle;
        buildParameters.CopyBuildinFileOption = AssetBundleBuilderSettingData.Setting.CopyBuildinFileOption;
        buildParameters.CopyBuildinFileTags = AssetBundleBuilderSettingData.Setting.CopyBuildinFileTags;

        if (AssetBundleBuilderSettingData.Setting.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
        {
            buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
            buildParameters.SBPParameters.WriteLinkXML = true;
        }

        var builder = new AssetBundleBuilder();
        var buildResult = builder.Run(buildParameters);
        if (buildResult.Success)
        {
            EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
        }
    }

    private static string GetBuildPackageVersion()
    {
        int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
        return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
    }

    private static IEncryptionServices CreateEncryptionServicesInstance()
    {
        int defaultIndex = GetDefaultEncryptionIndex(AssetBundleBuilderSettingData.Setting.EncyptionClassName);
        List<Type> _encryptionServicesClassTypes = GetEncryptionServicesClassTypes();
        var classType = _encryptionServicesClassTypes[defaultIndex];
        return (IEncryptionServices)Activator.CreateInstance(classType);
    }

    private static List<Type> GetEncryptionServicesClassTypes()
    {
        return EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
    }

    // 加密类相关
    private static int GetDefaultEncryptionIndex(string className)
    {
        List<Type> _encryptionServicesClassTypes = GetEncryptionServicesClassTypes();
        List<string> _encryptionServicesClassNames =_encryptionServicesClassTypes.Select(t => t.Name).ToList();
        for (int index = 0; index < _encryptionServicesClassNames.Count; index++)
        {
            if (_encryptionServicesClassNames[index] == className)
            {
                return index;
            }
        }

        AssetBundleBuilderSettingData.IsDirty = true;
        AssetBundleBuilderSettingData.Setting.EncyptionClassName = _encryptionServicesClassNames[0];
        return 0;
    }

    //[MenuItem("BuildTools/BuildAPK")]
    //public static void BuildAPK()
    //{
    //    BuildPlayerOptions options = new BuildPlayerOptions();
    //    string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
    //    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    //    {
    //        scenePaths[i] = EditorBuildSettings.scenes[i].path;
    //    }
    //    options.scenes = scenePaths;
    //    options.target = BuildTarget.Android;
    //    options.options = BuildOptions.None;
    //    //E:/MyFramework/my_framework/
    //    string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
    //    options.locationPathName = $"{projectPath}BuildApk/Apk.apk";
    //    BuildPipeline.BuildPlayer(options);
    //}

    private static BuildToolsConfig configInfo;
    private static string configPath;
    private void Awake()
    {
        configInfo = InitBuildToolsPath();
    }

    private static BuildToolsConfig InitBuildToolsPath()
    {
        configPath = Application.dataPath + "/Resources/BuildToolsConfig.txt";
        if (!File.Exists(configPath))
        {
            FileStream stream = File.Create(configPath);
            stream.Close();
        }
        string jsonStr = File.ReadAllText(configPath);
        BuildToolsConfig jsonData = new BuildToolsConfig();
        if (!string.IsNullOrEmpty(jsonStr))
        {
            jsonData = JsonMapper.ToObject<BuildToolsConfig>(jsonStr);
        }
        return jsonData;
    }

    private void OnGUI()
    {
        DrawBuildApp();
    }
    
    private void DrawBuildApp()
    {
        if (configInfo == null)
        {
            configInfo = InitBuildToolsPath();
        }

        GUILayout.BeginHorizontal();
        configInfo.assembly_SourcePath = $"{SettingsUtil.HybridCLRDataDir}/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget }";
        configInfo.assembly_SourcePath =configInfo.assembly_SourcePath.Replace("\\", "/");
        configInfo.assembly_SourcePath = EditorGUILayout.TextField("assembly_SourcePath(绝对路径)", configInfo.assembly_SourcePath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        configInfo.assembly_TargetPath=configInfo.assembly_TargetPath.Replace("\\", "/");
        configInfo.assembly_TargetPath = EditorGUILayout.TextField("assembly_TargetPath(绝对路径)", configInfo.assembly_TargetPath);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SavePath"))
        {
            //写入配置
            JsonWriter writer = new JsonWriter();
            writer.PrettyPrint = true;  //写入json时自动换行
            JsonMapper.ToJson(configInfo, writer);
            File.WriteAllText(configPath, writer.TextWriter.ToString());
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("移动HotfixUpdate.dll"))
        {
            MoveHotfixUpdateDll();
            Debug.Log("*** 移动HotfixUpdate完成  ****");
        }


        //if (GUILayout.Button("生成MD5文件"))
        //{
        //    // 打AssetBundle
        //    BuildAssetBundle.BuildABMD5List(configInfo);
        //    AssetDatabase.Refresh();
        //}

        //if (GUILayout.Button("生成压缩包"))
        //{
        //    //string zipDir = Application.streamingAssetsPath + "/HotfixRootPath";
        //    //BuildAssetBundle.ZipUpdateDir(zipDir, VersionMgr.instance.resVersion);
        //    //Debug.Log("生成压缩包成功");
        //    //AssetDatabase.Refresh();
        //}

        //if (GUILayout.Button("Build APP"))
        //{
        //    //// 生成原始lua全量文件的md5
        //    //BuildUtils.GenOriginalLuaFrameworkMD5File();

        //    //// 打AssetBundle
        //    ////BuildAssetBundle.Build();
        //    //BuildAssetBundle.BuildABMD5List(hotfixConfigInfo);

        //    //// 打包APP
        //    //BuildUtils.BuildApp();
        //}
    }

    private static void MoveHotfixUpdateDll()
    {
        if (configInfo == null)
        {
            configInfo = InitBuildToolsPath();
        }
        string sourceFilePath = configInfo.assembly_SourcePath + "/HotUpdate.dll";
        Debug.Log("sourceFilePath : " + sourceFilePath);
        if (File.Exists(sourceFilePath))
        {
            var directory = Path.GetDirectoryName(configInfo.assembly_TargetPath);
            if (!Directory.Exists(directory))
            {
                Debug.LogError("路径不存在 ： " + configInfo.assembly_TargetPath);
                return;
            }

            string sourceFileNewNamePath = configInfo.assembly_SourcePath + "/HotUpdate.dll.bytes";
            if (File.Exists(sourceFilePath))
            {
                File.Move(sourceFilePath, sourceFileNewNamePath);
            }

            if (File.Exists(sourceFileNewNamePath))
            {
                string targetFileNewNamePath = configInfo.assembly_TargetPath + "/HotUpdate.dll.bytes";
                File.Copy(sourceFileNewNamePath, targetFileNewNamePath, true);
            }
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// BuildToolsPanel配置信息
    /// </summary>
    public class BuildToolsConfig
    {
        public string assembly_SourcePath = "";
        public string assembly_TargetPath = "";
    }
}

