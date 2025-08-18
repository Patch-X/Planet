using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using LitJson;
using System.Collections;
using static BuildAB;
using System.Diagnostics;
using Ionic.Zip;


public class BuildAssetBundle
{
    private static string abExportPath;

    private static string subMD5FilesPath;
    private static Dictionary<string, MD5FilesInfo> subMD5FilesInfoDic = new Dictionary<string, MD5FilesInfo>();

    private static string md5FilesPath;
    private static Dictionary<string, MD5FilesInfo> md5FilesInfoDic = new Dictionary<string, MD5FilesInfo>();

    /// <summary>
    /// 打APK 打AB
    /// </summary>
    public static void Build()
    {
        string targetPath = Application.streamingAssetsPath + "/res";
        BuildUtils.BuildLuaBundle(targetPath);
        BuildUtils.BuildNormalCfgBundle(targetPath);
        BuildUtils.BuildGameResBundle(targetPath);
        Debuger.Log("BuildAssetBundle Done");
    }

    /// <summary>
    /// 创建MD5列表
    /// </summary>
    public static void BuildABMD5List(HotfixConfigInfo pathConfigInfo)
    {
        abExportPath = Application.streamingAssetsPath + "/HotfixRootPath";
        if (!Directory.Exists(abExportPath))
        {
            Directory.CreateDirectory(abExportPath);
        }

        subMD5FilesPath = abExportPath + "/SubMD5Files.txt";
        subMD5FilesInfoDic = new Dictionary<string, MD5FilesInfo>();
        if (!File.Exists(subMD5FilesPath))
        {
            FileStream stream = File.Create(subMD5FilesPath);
            stream.Close();
        }
        else
        {
            File.WriteAllText(subMD5FilesPath, null);
        }

        md5FilesPath = abExportPath + "/MD5Files.txt";
        //File.Delete(md5FilesPath);
        if (!File.Exists(md5FilesPath))
        {
            FileStream stream = File.Create(md5FilesPath);
            stream.Close();
        }
        string md5FilesJson = File.ReadAllText(md5FilesPath);
        md5FilesInfoDic = new Dictionary<string, MD5FilesInfo>();
        if (!string.IsNullOrEmpty(md5FilesJson))
        {
            md5FilesInfoDic = JsonMapper.ToObject<Dictionary<string, MD5FilesInfo>>(md5FilesJson);
        }


        //CreateAB(pathConfigInfo.cfg_Path, ResTypeName.Cfg);
        //Debuger.Log("cfg_Path  写入 MD5列表 Done");

        //CreateAB(pathConfigInfo.prefab_Path, ResTypeName.UIPrefab);
        //Debuger.Log("prefab_Path  写入 MD5列表 Done");

        //CreateLuaFilesAB(pathConfigInfo.luaFile_Path ,pathConfigInfo.isAESEncrypt, ResTypeName.Lua);
        //Debuger.Log("luaFile_Path  写入 MD5列表 Done");

        //CreateLuaFilesAB(pathConfigInfo.toluaFile_Path, pathConfigInfo.isAESEncrypt,ResTypeName.Lua);
        //Debuger.Log("toluaFile_Path  写入 MD5列表 Done");
    }

    /// <summary>
    /// 打配置表 AB
    /// </summary>
    static void CreateAB(string resPath,string folderName)
    {
        List<string> detMetaFileList = new List<string>();
        List<string> fileList = BuildUtils.GetFiles(Application.dataPath + "/" +resPath, true);
        for (int row = 0; row < fileList.Count; row++)
        {
            string path = fileList[row];
            if (path.Contains(".meta") == false)
            {
                path = path.Replace("\\", "/");
                path = path.Replace("//", "/");
                detMetaFileList.Add(path);
            }
        }

        List<string> changeFileList = new List<string>();
        for (int row = 0; row < detMetaFileList.Count; row++)
        {
            string path = detMetaFileList[row];
            string md5 = Util.md5file(path);
            long size = Util.GetFileSize(path);
            MD5FilesInfo mD5FilesInfo = new MD5FilesInfo();
            mD5FilesInfo.MD5 = md5;
            mD5FilesInfo.Size = size;
            if (md5FilesInfoDic.ContainsKey(path))
            {
                if (md5FilesInfoDic[path].MD5 != md5)
                {
                    //修改
                    md5FilesInfoDic[path] = mD5FilesInfo;
                    changeFileList.Add(path);
                    subMD5FilesInfoDic.Add(path, mD5FilesInfo);
                }
                else
                { }
            }
            else
            {
                md5FilesInfoDic.Add(path, mD5FilesInfo);
                changeFileList.Add(path);
                subMD5FilesInfoDic.Add(path, mD5FilesInfo);
            }
        }
        //打AB
        string abExportPathTemp = abExportPath + "/" + folderName;
        if (!Directory.Exists(abExportPathTemp))
        {
            Directory.CreateDirectory(abExportPathTemp);
        }
        for (int row = 0; row < changeFileList.Count; row++)
        {
            string path = changeFileList[row];
            string[] pathArray = path.Split('/');
            string fileName="";
            if (pathArray.Length > 0)
            {
                string fileNameAss = pathArray[pathArray.Length - 1];
                string[] strArray = fileNameAss.Split('.');
                if (strArray.Length == 2)
                {
                    fileName = strArray[0];
                }
                else
                {
                    UnityEngine.Debug.LogError("文件名错误，不能包含点(.)号");
                    return;
                }
            }
            Hashtable tb = new Hashtable();
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            assetBundle.assetBundleName = fileName + ".bundle";
            assetBundle.assetNames = new string[] { path };
            AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
            buildArray[0] = assetBundle;
            BuildUtils.BuildBundles(buildArray, abExportPathTemp);
        }


        //写入配置
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;  //写入json时自动换行
        JsonMapper.ToJson(md5FilesInfoDic, writer);
        File.WriteAllText(md5FilesPath, writer.TextWriter.ToString());

        //写入配置: 部分资源更新列表
        JsonWriter subWriter = new JsonWriter();
        subWriter.PrettyPrint = true;
        JsonMapper.ToJson(subMD5FilesInfoDic, subWriter);
        File.WriteAllText(subMD5FilesPath, subWriter.TextWriter.ToString());
    }

    static void CreateLuaFilesAB(string resPath, bool isAESEncrypt, string folderName)
    {
        resPath = resPath.Replace("\\", "/");
        string sourceFolder = Application.dataPath + "/" + resPath;

        // 创建Lua的Bundle临时目录
        string tempFolder = folderName+"/" + resPath;
        string tmpPath = BuildUtils.CreateTmpDir(tempFolder);
        BuildUtils.CopyFolder(sourceFolder, tmpPath);

        List<string> fileList = BuildUtils.GetFiles(tmpPath, true);
        if (fileList.Count == 0)
        {
            UnityEngine.Debug.LogError("没有Lua文件");
            return;
        }
        // 将Lua代码拷贝到Bundle临时目录（做加密处理）
        BuildUtils.CopyLuaToBundleDir(fileList, tmpPath, isAESEncrypt);

        List<string> detMetaFileList = new List<string>();
        List<string> aesFileList = BuildUtils.GetFiles(tmpPath, true);
        for (int row = 0; row < aesFileList.Count; row++)
        {
            string path = aesFileList[row];
            if (path.Contains(".meta") == false)
            {
                path = path.Replace("\\", "/");
                path = path.Replace("//", "/");
                detMetaFileList.Add(path);
            }
        }

        List<string> changeFileList = new List<string>();
        for (int row = 0; row < detMetaFileList.Count; row++)
        {
            string path = detMetaFileList[row];
            string md5 = Util.md5file(path);
            long size = Util.GetFileSize(path);
            MD5FilesInfo mD5FilesInfo = new MD5FilesInfo();
            mD5FilesInfo.MD5 = md5;
            mD5FilesInfo.Size = size;
            if (md5FilesInfoDic.ContainsKey(path))
            {
                if (md5FilesInfoDic[path].MD5 != md5)
                {
                    //修改
                    md5FilesInfoDic[path] = mD5FilesInfo;
                    changeFileList.Add(path);
                    subMD5FilesInfoDic.Add(path, mD5FilesInfo);
                }
                else
                { }
            }
            else
            {
                md5FilesInfoDic.Add(path, mD5FilesInfo);
                changeFileList.Add(path);
                subMD5FilesInfoDic.Add(path, mD5FilesInfo);
            }
        }

        //return;

        /* Lua不需要打AB
        //for (int row = 0; row < changeFileList.Count; row++)
        //{
        //    string path = changeFileList[row];
        //    string[] pathArray = path.Split('/');
        //    string fileName = "";
        //    if (pathArray.Length > 0)
        //    {
        //        string fileNameAss = pathArray[pathArray.Length - 1];
        //        string[] strArray = fileNameAss.Split('.');
        //        if (strArray.Length > 0)
        //        {
        //            fileName = strArray[0];
        //        }
        //    }
        //    Hashtable tb = new Hashtable();
        //    AssetBundleBuild assetBundle = new AssetBundleBuild();
        //    assetBundle.assetBundleName = fileName + ".bundle";
        //    assetBundle.assetNames = new string[] { path };
        //    AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        //    buildArray[0] = assetBundle;
        //    BuildUtils.BuildBundles(buildArray, abExportPath);
        //}
        //GameLogger.Log("打  AB列表 Done");
        */


        //Lua直接拷贝到文件夹里
        for (int row = 0; row < changeFileList.Count; row++)
        {
            string sourceFolderPath = Application.dataPath + "/../" + changeFileList[row];
            string[] pathNodeArray = changeFileList[row].Split('/');
            string fileFullPath = "";
            for (int col = 0; col < pathNodeArray.Length; col++)
            {
                if (col == 0)
                {
                }
                else if (col == pathNodeArray.Length - 1)
                {
                }
                else
                {
                    fileFullPath += pathNodeArray[col] + "/";
                }
            }

            string folderNameTemp = Path.GetFileName(changeFileList[row]);
            string newFolderPath = abExportPath + "/" + fileFullPath;
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }
            string destinationFilePath = Path.Combine(newFolderPath, Path.GetFileName(sourceFolderPath));
            File.Copy(sourceFolderPath, destinationFilePath, true); // true 表示如果目标文件存在则覆盖它
        }


        //写入配置
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;  
        JsonMapper.ToJson(md5FilesInfoDic, writer);
        File.WriteAllText(md5FilesPath, writer.TextWriter.ToString());

        //写入配置: 部分资源更新列表
        JsonWriter subWriter = new JsonWriter();
        subWriter.PrettyPrint = true;
        JsonMapper.ToJson(subMD5FilesInfoDic, subWriter);
        File.WriteAllText(subMD5FilesPath, subWriter.TextWriter.ToString());
    }

    public static void ZipUpdateDir(string zipDir, string resVersion)
    {
        /*
        string zipFilePath = AppConst.GetHotfixZipPath(resVersion);   //BuildUtils.BIN_PATH + "res_" + resVersion + ".zip";
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        ZipFile zipFile = new ZipFile();
        zipFile.AddDirectory(zipDir);
        zipFile.Save(zipFilePath);
        zipFile.Dispose();
        */
    }
    class MD5FilesInfo
    {
        public string MD5;
        public long Size;
    } 
}


