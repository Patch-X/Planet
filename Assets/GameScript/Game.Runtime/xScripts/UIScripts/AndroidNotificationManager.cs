using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
#endif
using System;
public class AndroidNotificationManager : MonoBehaviour
{
    // 单例模式
    public static AndroidNotificationManager Instance { get; private set; }
#if UNITY_ANDROID && !UNITY_EDITOR
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
        AndroidNotificationCenter.RegisterNotificationChannel(channel);//注册频道

    }

    public int ScheduleNotification(DateTime firetime, string message, bool isDaily = false)
    {
        // // 计算通知触发时间
        // System.DateTime triggerTime = CalculateTriggerTime(hour, minute);
        var notification = new AndroidNotification//创建一条通知
        {
            Title = "Reminder",
            Text = message,
            FireTime = firetime,
            SmallIcon = "ic_notification", // 需要在Android资源中定义
            ShowTimestamp = true,//显示时间戳
            ShowInForeground = true,

        };
        return AndroidNotificationCenter.SendNotification(notification, "reminder_channel");// 发送通知，返回生成的通知ID

    }
    public bool CheckPermissionStatus()//检查用户权限
    {

        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.Allowed)//判断用户发布权限
        {
            if (!AndroidNotificationCenter.IgnoringBatteryOptimizations)//应用是否忽略设备电池优化设置
            {
                AndroidNotificationCenter.RequestIgnoreBatteryOptimizations();//请求忽略电池优化
                // AndroidNotificationCenter.RequestExactScheduling();//请求精确调度
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            AndroidNotificationCenter.OpenNotificationSettings(); //打开通知设置
            return false;
        }
    }
    public bool CheckPermissionStatustostring()//检查用户权限
    {

        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.Allowed)//判断用户发布权限
        {
            if (AndroidNotificationCenter.IgnoringBatteryOptimizations)
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    // // 计算触发时间（处理跨天情况）
    // private System.DateTime CalculateTriggerTime(int hour, int minute)
    // {
    //     System.DateTime now = System.DateTime.Now;
    //     System.DateTime triggerTime = new System.DateTime(
    //         now.Year, now.Month, now.Day, hour, minute, 0);

    //     // 如果时间已过，设置为明天
    //     if (triggerTime <= now)
    //     {
    //         triggerTime = triggerTime.AddDays(1);
    //     }

    //     return triggerTime;
    // }

    // 取消特定通知
    public void CancelNotification(int notificationId)
    {
        AndroidNotificationCenter.CancelScheduledNotification(notificationId);

    }

    public string CheckNotificationStatus(int id)//检查通知状态
    {
        NotificationStatus status = AndroidNotificationCenter.CheckScheduledNotificationStatus(id);
        switch (status)
        {
            case NotificationStatus.Scheduled:
                return "Scheduled";
            case NotificationStatus.Delivered:
                return "Delivered";
            case NotificationStatus.Unknown:
                return "Unknown";
            case NotificationStatus.Unavailable:
                return "Unavailable";
        }
        return "";

    }
    #endif
}