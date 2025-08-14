using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;
using System.Collections.Generic;

public class VersionGUI 
{
    public void Awake()
    {
        //VersionMgr.instance.Init();
        //app_Version = VersionMgr.instance.appVersion;
        //res_version = VersionMgr.instance.resVersion;  //初始时读取本地版本号，之后都从热更缓存里读取版本号
    }

    public void DrawVersion()
    {
        /*
        GUILayout.BeginHorizontal();
        app_Version = EditorGUILayout.TextField("app_Version", app_Version);
        res_version = EditorGUILayout.TextField("res_version", res_version);
        JsonData jd = new JsonData();
        jd["app_version"] = app_Version;       //从"/Resources/version.bytes"中读取
        jd["res_version"] = res_version;     
        if (GUILayout.Button("Save"))
        {
            using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/version.bytes"))
            {
                sw.Write(jd.ToJson());
            }
            AssetDatabase.Refresh();
            Debug.Log("Save Version OK: " + app_Version);
            VersionMgr.instance.DeleteCacheResVersion();
            VersionMgr.instance.Init();
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("修改更新配置(复制压缩包)"))
        {
            DrawChangeHotConfig();
        }
        GUILayout.EndHorizontal();
        */
    }

    /// <summary>
    /// 修改热更配置
    /// </summary>
    void DrawChangeHotConfig()
    {
        /*
        string appVersion = VersionMgr.instance.appVersion;
        string resVersion = VersionMgr.instance.resVersion;
        string zipName = "res_" + resVersion + ".zip";
        string zipFilePath = AppConst.GetHotfixZipPath(resVersion);//BuildUtils.BIN_PATH + zipName;
        var zipMd5 = LuaFramework.Util.md5file(zipFilePath);
        var zipSize = LuaFramework.Util.GetFileSize(zipFilePath);
        //移动文件到指定目录
        LuaFramework.Util.CopyFileToFolder(zipFilePath, AppConst.WebUrl_LocalTest);

        string hotfixConfigPath = AppConst.WebUrl_LocalTest + "update_list.json";
        string json = File.ReadAllText(hotfixConfigPath);
        List<UpdateInfo> updateInfoList = JsonMapper.ToObject<List<UpdateInfo>>(json);
        updateInfoList.Sort((a, b) =>
        {
            //appVersion  小到大排列
            return -VersionMgr.CompareVersion(b.appVersion, a.appVersion);
        });

        Dictionary<string, UpdateInfo> dicUpdateInfo = new Dictionary<string, UpdateInfo>();
        for (int row = 0; row < updateInfoList.Count; row++)
        {
            UpdateInfo updateInfoTemp = updateInfoList[row];
            dicUpdateInfo.Add(updateInfoTemp.appVersion, updateInfoTemp);
        }

        UpdateInfo updateInfo = null;
        if (dicUpdateInfo.ContainsKey(appVersion))
        {
            updateInfo = dicUpdateInfo[appVersion];
        }
        else
        {
            updateInfo = new UpdateInfo();
            updateInfo.updateList = new List<PackInfo>();
            updateInfoList.Add(updateInfo);
        }
        updateInfo.appVersion = appVersion;
        updateInfo.appUrl = VersionMgr.instance.appUrl;
        updateInfo.updateList.Sort((a, b) =>
        {
            //resVersion  小到大排列
            return -VersionMgr.CompareVersion(b.resVersion, a.resVersion);
        });

        //添加PackInfo
        {
            Dictionary<string, PackInfo> packInfoDic = new Dictionary<string, PackInfo>();
            for (int row = 0; row < updateInfo.updateList.Count; row++)
            {
                PackInfo packInfoTemp = updateInfo.updateList[row];
                packInfoDic.Add(packInfoTemp.resVersion, packInfoTemp);
            }

            PackInfo packInfo = null;
            if (packInfoDic.ContainsKey(resVersion))
            {
                packInfo = packInfoDic[resVersion];
            }
            else
            {
                packInfo = new PackInfo();
                updateInfo.updateList.Add(packInfo);
            }
            packInfo.resVersion = resVersion;
            packInfo.md5 = zipMd5;
            packInfo.size = zipSize;
            packInfo.url = AppConst.WebUrl_RemoveTest + zipName;
        }

        //写入配置
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;  //写入json时自动换行
        JsonMapper.ToJson(updateInfoList, writer);
        File.WriteAllText(hotfixConfigPath, writer.TextWriter.ToString());

    */
    }


    public string app_Version;
    public string res_version;
}
