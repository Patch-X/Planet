//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UniFramework.Pooling;
//using UniFramework.Window;
//using UniFramework.Machine;
//using UniFramework.Singleton;
//using YooAsset;
//using System;
//using System.Reflection;
//using System.Linq;
//using MotionFramework.Resource;
//using HybridCLR;

//internal class FsmLoadHotfixDll : IStateNode
//{
//	private StateMachine _machine;
//    private static Assembly _hotUpdateAss;
//    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
//    {
//        "mscorlib.dll",
//        "System.dll",
//        "System.Core.dll",
//    };

//    void IStateNode.OnCreate(StateMachine machine)
//	{
//		_machine = machine;
//	}
//	void IStateNode.OnEnter()
//	{
//        LoadHotfixDll();
//    }
//	void IStateNode.OnUpdate()
//	{
//	}
//	void IStateNode.OnExit()
//	{
//	}

//	private void LoadHotfixDll()
//    {
//        LoadMetadataForAOTAssemblies();

//        //#if !UNITY_EDITOR
//        //        //_hotUpdateAss = Assembly.Load(ReadBytesFromStreamingAssets("HotUpdate.dll.bytes"));
//        //        RawFileOperationHandle _handle = YooAssets.LoadRawFileSync("HotUpdate.dll.bytes");
//        //        _hotUpdateAss = Assembly.Load(_handle.GetRawFileData());
//        //#else
//        //        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
//        //#endif
//        RawFileOperationHandle _handle = YooAssets.LoadRawFileSync("HotUpdate.dll");
//        _hotUpdateAss = Assembly.Load(_handle.GetRawFileData());
//        Type entryType = _hotUpdateAss.GetType("Hello");
//        entryType.GetMethod("Run").Invoke(null, null);

//        _machine.ChangeState<FsmInitGame>();
//    }

//    /// <summary>
//    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
//    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
//    /// </summary>
//    private static void LoadMetadataForAOTAssemblies()
//    {
//        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
//        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
//        /// 
//        HomologousImageMode mode = HomologousImageMode.SuperSet;
//        foreach (var aotDllName in AOTMetaAssemblyFiles)
//        {
//            //byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
//            //// 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
//            //LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
//            //Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");

//            RawFileOperationHandle _handle = YooAssets.LoadRawFileSync(aotDllName); /// LoadRawFileSync(aotDllName);
//            byte[] dllBytes = _handle.GetRawFileData();
//            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
//            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
//        }
//    }
//}