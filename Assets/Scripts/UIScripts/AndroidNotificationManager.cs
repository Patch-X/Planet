using UnityEngine;
using Unity.Notifications.Android;

public class AndroidNotificationManager : MonoBehaviour
{
    // 单例模式
    public static AndroidNotificationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保该游戏对象在场景切换时不会被销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeNotificationChannel();
    }

    // 初始化通知频道（Android 8.0+ 必需）
    private void InitializeNotificationChannel()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        var channel = new AndroidNotificationChannel
        {
            Id = "reminder_channel",
            Name = "定时提醒",
            Description = "用于显示定时提醒通知",
            Importance = Importance.High,  // 高优先级会显示横幅通知
            EnableLights = true,// 是否显示呼吸灯
            EnableVibration = true,// 是否震动
            CanBypassDnd = true,//可以绕过拒绝服务
            CanShowBadge = true,//启用角标


        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

    }


    /// <summary>
    /// 设置定时通知
    /// </summary>
    /// <param name="hour">小时 (0-23)</param>
    /// <param name="minute">分钟 (0-59)</param>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知内容</param>
    /// <param name="id">通知ID（用于取消特定通知）</param>
    /// <param name="isDaily">是否每日重复</param>
    public int ScheduleNotification(int hour, int minute, string message, bool isDaily = false)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.LogWarning("当前平台不是Android，无法发送通知");
            // return;
        }

        // 计算通知触发时间
        System.DateTime triggerTime = CalculateTriggerTime(hour, minute);
        Debug.Log(hour + +minute + message);

        var notification = new AndroidNotification//创建一条通知
        {
            Title = "Reminder",
            Text = message,
            FireTime = triggerTime,
            SmallIcon = "ic_notification", // 需要在Android资源中定义
            ShowTimestamp = true,//显示时间戳
            ShowInForeground = true,


        };


        return AndroidNotificationCenter.SendNotification(notification, "reminder_channel");// 发送通知，返回生成的通知ID


        // // 如需每日重复，可使用AlarmManager替代
        // if (isDaily)
        // {
        //     // 这里需要使用AndroidJavaClass调用原生AlarmManager
        //     // 或在每次通知触发后重新调度下一次通知
        // }
    }
    public bool CheckPermissionStatus()//检查用户权限
    {
        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.Allowed)//判断用户发布权限
        {
            if (!AndroidNotificationCenter.IgnoringBatteryOptimizations)//应用是否忽略设备电池优化设置
            {
                AndroidNotificationCenter.RequestIgnoreBatteryOptimizations();//请求忽略电池优化
                AndroidNotificationCenter.RequestExactScheduling();//请求精确调度
                return false;


            }
            else
            {

                return true;
            }

        }
        // else if (AndroidNotificationCenter.ShouldShowPermissionToPostRationale)
        // {
        //     Debug.Log("需要打开通知");//提示用户打开通知
        // }
        else
        {
            AndroidNotificationCenter.OpenNotificationSettings(); //请求权限
            // var request = new PermissionRequest();//请求权限安卓13以上
            return false;
        }
    }

    // 计算触发时间（处理跨天情况）
    private System.DateTime CalculateTriggerTime(int hour, int minute)
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime triggerTime = new System.DateTime(
            now.Year, now.Month, now.Day, hour, minute, 0);

        // 如果时间已过，设置为明天
        if (triggerTime <= now)
        {
            triggerTime = triggerTime.AddDays(1);
        }

        return triggerTime;
    }

    // 取消特定通知
    public void CancelNotification(int notificationId)
    {
        AndroidNotificationCenter.CancelScheduledNotification(notificationId);

    }

    public string CheckNotificationStatus(int id)//检查通知状态
    {
        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Delivered)//通知已经送达
        {

            return "Delivered";
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)//通知发送失败
        {

            return "Scheduled";
        }
        else
        {
            Debug.Log("Unavailable");
            return "Unavailable";
        }

    }
    private int GetAndroidMajorVersion()
    {
        if (Application.platform != RuntimePlatform.Android)
            return -1; // 非 Android 平台返回 -1

        string osString = SystemInfo.operatingSystem;
        string[] parts = osString.Split(' ');

        if (parts.Length < 2 || !parts[0].Equals("Android"))
            return -1; // 格式不匹配

        string versionPart = parts[1];
        string[] versionSegments = versionPart.Split('.');

        if (int.TryParse(versionSegments[0], out int majorVersion))
            return majorVersion;

        return -1; // 解析失败
    }

}