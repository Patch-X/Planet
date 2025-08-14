
using System.Collections;
using UniFramework.Pooling;
using UniFramework.Singleton;
using UniFramework.Window;
using UnityEngine;
using YooAsset;

public class Entry
{
    public static void Run()
    {
        Debuger.Log("Hello, HybridCLR  ==>Test4");
        Debuger.Log("开始执行热更代码 3333");

        // 创建游戏管理器
        UniSingleton.CreateSingleton<GameManager>();
        // 开启游戏流程
        GameManager.Instance.Run();
    }
}