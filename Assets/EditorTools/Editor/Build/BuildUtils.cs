using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using System.Security.Cryptography;
using System;
using System.Text;

public class BuildUtils
{

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 获取文件的大小
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static long GetFileSize(string filePath)
    {
        //判断当前路径所指向的是否为文件
        if (File.Exists(filePath))
        {
            //定义一个FileInfo对象,使之与filePath所指向的文件向关联,
            //以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        else
        {
            return -1;
        }
    }

    public static void CopyFileToFolder(string sourceFilePath, string destinationFolderPath)
    {
        // 确保目标文件夹存在
        if (File.Exists(sourceFilePath) == false)
        {
            Debuger.LogError("源文件夹不存在");
            return;
        }
        if (Directory.Exists(destinationFolderPath) == false)
        {
            Debuger.LogError("目标路径不存在");
            return;
        }

        //Directory.CreateDirectory(destinationFolderPath);
        // 获取源文件和目标文件的路径
        string fileName = Path.GetFileName(sourceFilePath);
        string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
        // 复制文件
        File.Copy(sourceFilePath, destinationFilePath, true); // true 表示如果目标文件存在，则覆盖它
        Debuger.Log(string.Format("拷贝{0}文件到{1}路径下完成", sourceFilePath, destinationFilePath));
    }

    /// <summary>
    /// 递归遍历获取目标目录中的所有文件
    /// </summary>
    /// <param name="sourceDir">目标目录</param>
    /// <param name="splitAssetPath">是否要切割目录，以Assets目录为根</param>
    public static List<string> GetFiles(string sourceDir, bool splitAssetPath)
    {
        List<string> fileList = new List<string>();
        string[] fs = Directory.GetFiles(sourceDir);//获取当前目录下的所有文件
        string[] ds = Directory.GetDirectories(sourceDir);//获取当前目录下的所有子目录方法
        for (int i = 0, len = fs.Length; i < len; ++i)
        {
            var index = splitAssetPath ? fs[i].IndexOf("Assets") : 0;
            fileList.Add(fs[i].Substring(index));
        }
        for (int i = 0, len = ds.Length; i < len; ++i)
        {
            fileList.AddRange(GetFiles(ds[i], splitAssetPath));
        }
        return fileList;
    }

    public static List<string> GetFiles(string[] sourceDirs, bool splitAssetPath)
    {
        List<string> fileList = new List<string>();
        foreach (var sourceDir in sourceDirs)
        {
            fileList.AddRange(GetFiles(sourceDir, splitAssetPath));
        }
        return fileList;
    }



    /// <summary>
    /// 根据哈希表构建AssetBundleBuild列表
    /// </summary>
    /// <param name="tb">哈希表，key为assetBundleName，value为目录</param>
    /// <returns></returns>
    public static AssetBundleBuild[] MakeAssetBundleBuildArray(Hashtable tb)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[tb.Count];
        int index = 0;
        foreach (string key in tb.Keys)
        {
            buildArray[index].assetBundleName = key;
            List<string> fileList = new List<string>();
            fileList = GetFiles(Application.dataPath + "/" + tb[key], true);
            buildArray[index].assetNames = fileList.ToArray();
            ++index;
        }

        return buildArray;
    }

    /// <summary>
    /// 打包常规配置表AssetBundle
    /// </summary>
    public static void BuildNormalCfgBundle(string targetPath)
    {
        Hashtable tb = new Hashtable();
        tb["normal_cfg.bundle"] = "GameRes/Config";
        AssetBundleBuild[] buildArray = BuildUtils.MakeAssetBundleBuildArray(tb);
        BuildUtils.BuildBundles(buildArray, targetPath);
    }

    /// <summary>
    /// 打包Lua的AssetBundle
    /// </summary>
    public static void BuildLuaBundle(string targetPath)
    {
        // 创建Lua的Bundle临时目录
        var luabundleDir = BuildUtils.CreateTmpDir("luabundle");
        // 将Lua代码拷贝到Bundle临时目录（做加密处理）
        var luaFiles = BuildUtils.GetFiles(new string[] {
            Application.dataPath + "/LuaFramework/Lua",
            Application.dataPath + "/LuaFramework/ToLua/Lua",
        }, true);
        BuildUtils.CopyLuaToBundleDir(luaFiles, luabundleDir);
        // 构建AssetBundleBuild列表
        Hashtable tb = new Hashtable();
        tb["lua.bundle"] = "luabundle";
        AssetBundleBuild[] buildArray = MakeAssetBundleBuildArray(tb);
        // 打包AssetBundle
        BuildBundles(buildArray, targetPath);

        // 删除Lua的Bundle临时目录
        DeleteDir(luabundleDir);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 打包游戏资源AssetBundle
    /// </summary>
    public static void BuildGameResBundle(string targetPath)
    {
        Hashtable tb = new Hashtable();
        tb["baseres.bundle"] = "GameRes/BaseRes";
        tb["uiprefabs.bundle"] = "GameRes/UIPrefabs";
        AssetBundleBuild[] buildArray = MakeAssetBundleBuildArray(tb);
        BuildBundles(buildArray, targetPath);
    }

    /// <summary>
    /// 打AssetBundle
    /// </summary>
    /// <param name="buildArray">AssetBundleBuild列表</param>
    public static void BuildBundles(AssetBundleBuild[] buildArray, string targetPath)
    {
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        BuildPipeline.BuildAssetBundles(targetPath, buildArray, BuildAssetBundleOptions.ChunkBasedCompression, GetBuildTarget());
        
    }

    public static void BuildApp()
    {
        /*
        string[] scenes = new string[] { "Assets/Scenes/Main.unity" };
        string appName = PlayerSettings.productName + "_" + VersionMgr.instance.appVersion + GetTargetPlatfromAppPostfix();
        string outputPath = Application.dataPath + "/../Bin/";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        string appPath = Path.Combine(outputPath, appName);

        // 根据你的需求设置各种版本号
        // PlayerSettings.Android.bundleVersionCode
        // PlayerSettings.bundleVersion
        // PlayerSettings.iOS.buildNumber

        BuildPipeline.BuildPlayer(scenes, appPath, GetBuildTarget(), BuildOptions.None);
        Debuger.Log("Build APP Done");
        */
    }

    /// <summary>
    /// 生成原始lua代码的md5
    /// </summary>
    public static void GenOriginalLuaFrameworkMD5File()
    {
        /*
        VersionMgr.instance.Init();
        JsonData jd = GetOriginalLuaframeworkMD5Json();
        var jsonStr = JsonMapper.ToJson(jd);
        jsonStr = jsonStr.Replace(",", ",\n");
        if (!Directory.Exists(BIN_PATH))
        {
            Directory.CreateDirectory(BIN_PATH);
        }
        using (StreamWriter sw = new StreamWriter(BIN_PATH + "LuaFrameworkFiles_" + VersionMgr.instance.appVersion + ".json"))
        {
            sw.Write(jsonStr);
        }
        Debuger.Log("GenLuaframeworkMd5 Done");
        */
    }

    public static JsonData GetOriginalLuaframeworkMD5Json()
    {
        var sourceDirs = new string[] {
            Application.dataPath + "/LuaFramework/Lua",
            Application.dataPath + "/LuaFramework/ToLua/Lua",
        };
        JsonData jd = new JsonData();
        /*
        foreach (var sourceDir in sourceDirs)
        {
            List<string> fileList = new List<string>();
            fileList = GetFiles(sourceDir, false);
            foreach (var luaFile in fileList)
            {
                if (!luaFile.EndsWith(".lua")) continue;

                var md5 = LuaFramework.Util.md5file(luaFile);
                var key = luaFile.Substring(luaFile.IndexOf("Assets/"));
                jd[key] = md5;
            }
        }
        */
        return jd;
    }

    /// <summary>
    /// 创建临时目录
    /// </summary>
    /// <returns>folderName</returns>文件夹名字
    public static string CreateTmpDir(string folderName)
    {
        var tmpPath = string.Format(Application.dataPath + "/{0}/", folderName);
        if (Directory.Exists(tmpPath))
        {
            Directory.Delete(tmpPath, true);
        }
        Directory.CreateDirectory(tmpPath);
        return tmpPath;
    }


    /// <summary>
    /// 拷贝文件夹到另一个文件夹
    /// </summary>
    /// <param name="sourceFolder"></param>
    /// <param name="destinationFolder"></param>
    public static void CopyFolder(string sourceFolder, string destinationFolder)
    {
        // 如果目标文件夹不存在，则创建
        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }

        // 获取源文件夹中的所有文件和子文件夹
        foreach (string filePath in Directory.GetFiles(sourceFolder))
        {
            // 获取文件名
            string fileName = Path.GetFileName(filePath);
            // 合并目标路径和文件名
            string destFilePath = Path.Combine(destinationFolder, fileName);
            // 复制文件到目标路径
            File.Copy(filePath, destFilePath, true); // true 表示如果目标文件存在则覆盖它
        }

        foreach (string folder in Directory.GetDirectories(sourceFolder))
        {
            // 获取子文件夹的名称
            string folderName = Path.GetFileName(folder);
            // 合并目标路径和子文件夹名称
            string destSubFolderPath = Path.Combine(destinationFolder, folderName);
            // 递归复制子文件夹
            CopyFolder(folder, destSubFolderPath);
        }
    }

    /// <summary>
    /// 删除目录
    /// </summary>
    public static void DeleteDir(string targetDir)
    {
        if (Directory.Exists(targetDir))
        {
            Directory.Delete(targetDir, true);
        }
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 拷贝Lua到目标目录，并做加密处理
    /// </summary>
    /// <param name="sourceDirs">源目录列表</param>
    /// <param name="luabundleDir">母包目录</param>
    public static void CopyLuaToBundleDir(List<string> luaFiles, string luabundleDir)
    {
        /*
        foreach (var luaFile in luaFiles)
        {
            if (luaFile.EndsWith(".meta")) continue;
            var luaFileFullPath = Application.dataPath + "/../" + luaFile;
            // 由于Build AssetBundle不识别.lua文件，所以拷贝一份到临时目录，统一加上.bytes结尾
            var targetFile = luaFile.Replace("Assets/LuaFramework/Lua", "");
            targetFile = targetFile.Replace("Assets/LuaFramework/ToLua/Lua", "");
            targetFile = luabundleDir + targetFile + ".bytes";
            var targetDir = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            // 做下加密
            byte[] bytes = File.ReadAllBytes(luaFileFullPath);
            byte[] encryptBytes = AESEncrypt.Encrypt(bytes);
            File.WriteAllBytes(targetFile, encryptBytes);
        }
        AssetDatabase.Refresh();
        */
    }


    public static void CopyLuaToBundleDir(List<string> luaFiles, string luabundleDir, bool isAESEncrypt = true)
    {
        /*
        luabundleDir = luabundleDir.Replace("\\", "/");
        foreach (var luaFile in luaFiles)
        {
            if (luaFile.EndsWith(".meta")) continue;
            var oldFileName = Application.dataPath + "/../" + luaFile;
            // 由于Build AssetBundle不识别.lua文件，所以拷贝一份到临时目录，统一加上.bytes结尾
            string[] array = luaFile.Split('/');
            string fileName = "";
            if (array.Length > 0)
            {
                fileName = array[array.Length-1];
            }
            string newFileName = luabundleDir + fileName + ".bytes";

            File.Move(oldFileName, newFileName);
            // 做下加密
            byte[] bytes = File.ReadAllBytes(newFileName);
            if (isAESEncrypt)
            {
                byte[] encryptBytes = AESEncrypt.Encrypt(bytes);
                File.WriteAllBytes(newFileName, encryptBytes);
            }
            else
            {
                File.WriteAllBytes(newFileName, bytes);
            }
        }
        AssetDatabase.Refresh();
        */
    }

    public void BuildLuaUpdateBundle(List<string> luaFileList)
    {

    }

    /// <summary>
    /// 获取当前平台
    /// </summary>
    public static BuildTarget GetBuildTarget()
    {

#if UNITY_STANDALONE
        return BuildTarget.StandaloneWindows;
#elif UNITY_ANDROID
        return BuildTarget.Android;
#else
        return BuildTarget.iOS;
#endif
    }

    /// <summary>
    /// 获取目标平台APP后缀
    /// </summary>
    /// <returns></returns>
    public static string GetTargetPlatfromAppPostfix(bool useAAB = false)
    {

#if UNITY_STANDALONE
        return ".exe";
#elif UNITY_ANDROID
        if(useAAB)
        {
            return ".aab";
        }
        else
        {
            return ".apk";
        }
#else
        return ".ipa";
#endif
    }

    public static string BIN_PATH
    {
        //get { return Application.dataPath + "/../Bin/"; }
        get { return Application.dataPath + "/Bin/"; }
    }
}
