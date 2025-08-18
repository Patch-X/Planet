using UnityEngine;
using UniFramework.Event;
using UniFramework.Singleton;
using YooAsset;

//1. 编译dll 2.移动dll到GameRes 3. 打增量包 4. 替换增量包资源
public class Launch : MonoBehaviour
{
	/// <summary>
	/// 资源系统运行模式
	/// </summary>
	public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

	void Awake()
    {
        //C:\Users\ASUS\AppData\LocalLow\DefaultCompany\MotionFramework
        Debuger.EnableHiDebugLogs(true);
        Debuger.EnableOnText(true);
        Debuger.EnableOnScreen(true);


        Debug.Log($"资源系统运行模式：{PlayMode}");
		Application.targetFrameRate = 60;
		Application.runInBackground = true;
	}
	void Start()
	{
		// 初始化事件系统
		//UniEvent.Initalize();

		// 初始化单例系统
		UniSingleton.Initialize();

		// 初始化资源系统
		YooAssets.Initialize();
		YooAssets.SetOperationSystemMaxTimeSlice(30);

		// 创建补丁管理器
		UniSingleton.CreateSingleton<PatchManager>();

		// 开始补丁更新流程
		PatchManager.Instance.Run(PlayMode);

    }
}