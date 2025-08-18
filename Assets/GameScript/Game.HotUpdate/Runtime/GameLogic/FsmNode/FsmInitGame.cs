using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Pooling;
using UniFramework.Window;
using UniFramework.Machine;
using UniFramework.Singleton;
using YooAsset;

internal class FsmInitGame : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		UniSingleton.StartCoroutine(Prepare());
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}

	private IEnumerator Prepare()
    {
        Debuger.Log("暂时不加载 加载UICanvas ...");
        //var handle = YooAssets.LoadAssetAsync<GameObject>("UICanvas");
        //yield return handle;
        //Debuger.Log("加载UICanvas完成 ...");
        //var canvas = handle.InstantiateSync();
		//var desktop = canvas.transform.Find("Desktop").gameObject;
		//GameObject.DontDestroyOnLoad(canvas);
		// 初始化窗口系统
		//UniWindow.Initalize(desktop);

		// 初始化对象池系统
		UniPooling.Initalize();

        ConfigManager.Instance.Init();

        _machine.ChangeState<FsmSceneBattle>();
		yield return null;
	}
}