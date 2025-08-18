using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Singleton;
using System.Reflection;
using YooAsset;
using HybridCLR;
using System;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmPatchDone : IStateNode
{
	void IStateNode.OnCreate(StateMachine machine)
	{
	}
	void IStateNode.OnEnter()
	{
		PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");
        Debug.Log("开始游戏");
        Debuger.Log("开始游戏...");

        LoadHotfixDll();
    }
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}

    private static Assembly _hotUpdateAss;
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    private void LoadHotfixDll()
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        //_hotUpdateAss = Assembly.Load(ReadBytesFromStreamingAssets("HotUpdate.dll.bytes"));
        RawFileOperationHandle _handle = YooAssets.LoadRawFileSync("HotUpdate.dll.bytes");
        _hotUpdateAss = Assembly.Load(_handle.GetRawFileData());
#else
        Assembly[] assemblyArray = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int row = 0; row < assemblyArray.Length; row++)
        {
            if(assemblyArray[row].GetName().Name == "HotUpdate")
            {
                _hotUpdateAss = assemblyArray[row];
                break;
            }
        }
#endif
        //unity编辑器里是不能断点的
        //RawFileOperationHandle _handle = YooAssets.LoadRawFileSync("HotUpdate.dll");
        //_hotUpdateAss = Assembly.Load(_handle.GetRawFileData());


        Type entryType = _hotUpdateAss.GetType("Entry");
        entryType.GetMethod("Run").Invoke(null, null);

        //代码需要注释掉：在Hello.Run的后边已经调用了 FsmInitGame
        //_machine.ChangeState<FsmInitGame>();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            //byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            //// 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            //LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            //Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");

            RawFileOperationHandle _handle = YooAssets.LoadRawFileSync(aotDllName); /// LoadRawFileSync(aotDllName);
            byte[] dllBytes = _handle.GetRawFileData();
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}