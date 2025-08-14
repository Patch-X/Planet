using UnityEngine;

public class AndroidSettingsPermissions : MonoBehaviour
{
    public void OnAPPLICATION_DETAILS_SETTINGS()//跳转应用详情设置
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");//获取com.unity3d.player.UnityPlayer类
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");//获取名为 currentActivity 的静态变量。
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");//获取android.content.Intent类对象
        // 设置Action：应用详情页面
        intent.Call<AndroidJavaObject>("setAction", "android.settings.APPLICATION_DETAILS_SETTINGS");//调用intent方法setAction，指定执行的动作是查看应用详情
        string packageName = currentActivity.Call<string>("getPackageName");//获取当前应用的包名
        // 指定要查看的应用包名
        AndroidJavaObject uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);//使用android.net.Uri类的fromParts方法创建一个URI对象，指定要查看的应用包名
        intent.Call<AndroidJavaObject>("setData", uri);//通过URI指定 “要查看权限的应用是哪个
        currentActivity.Call("startActivity", intent);//启动目标页面的 “执行命令”
    }
}
