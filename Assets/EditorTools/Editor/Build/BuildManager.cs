
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

//public class BuildManager
//{
//    [MenuItem("BuildManaer/BuildAPK")]
//    public static void BuildAPK()
//    {
//        BuildPlayerOptions options = new BuildPlayerOptions();
//        string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
//        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
//        {
//            scenePaths[i] = EditorBuildSettings.scenes[i].path;
//            Debug.Log(scenePaths[i]);
//        }
//        options.scenes = scenePaths;

//        options.target = BuildTarget.Android;
//        options.options = BuildOptions.None;
//        options.locationPathName = $"{SettingsUtil.HybridCLRDataDir}/BuildApk/Apk.apk";

//        BuildPipeline.BuildPlayer(options);
//    }
//}