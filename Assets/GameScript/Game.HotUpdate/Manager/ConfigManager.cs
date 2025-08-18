using Luban;
using System.IO;
using UnityEngine;
using YooAsset;

public class ConfigManager
{
    private static ConfigManager instance = null;

    public static ConfigManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConfigManager();
            }

            return instance;
        }
    }

    //public TbWeekRankMgr TbWeekRankMgr;

    public void Init()
    {
        cfg.Tables tables = new cfg.Tables(LoadByteBuf);
        //DebugToMgr.ToDebug("tables:" + tables.TbGift.DataList.Count);
        //TbWeekRankMgr = new TbWeekRankMgr(tables.TbWeekRank);
        //TestRead(tables);
    }

    void TestRead(cfg.Tables tables)
    {
        string content = "";
        foreach (var item in tables.TbItem.DataMap)
        {
            Debug.Log(item.Key + "  :  " + item.Value);
            content += item.Key + "  :  " + item.Value + "\n";
        }
        if (content != null)
        {
            UnityEngine.Debug.Log("content==>"+ content);
        }
    }

    private ByteBuf LoadByteBuf(string fileName)
    {
        RawFileOperationHandle _handle = YooAssets.LoadRawFileSync(fileName);
        byte[] byteArray = _handle.GetRawFileData();
        return new ByteBuf(byteArray);
        //return new ByteBuf(File.ReadAllBytes($"{Application.streamingAssetsPath}/Config/Bytes/{fileName}.bytes"));
    }
}

#region ±Ìπ‹¿Ì∆˜

//public class TbWeekRankMgr
//{
//    public TbWeekRank tables;

//    public TbWeekRankMgr(TbWeekRank tables)
//    {
//        this.tables = tables;
//    }
//}

#endregion