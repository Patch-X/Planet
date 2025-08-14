using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
#endif

public class ReminderManager : MonoBehaviour
{
    public GameObject ReminderPrefab; // 记事条预制体
    public GameObject OutReminderPrefab;
    public Transform ReminderList;     // 记事条列表的父物体
    public Transform OutReminderList;
    public InputField inputField;      // 输入框
    public Button submitButton;         // 提交按钮
    public Dropdown hourDropdown;      // 小时下拉菜单
    public Dropdown minuteDropdown;    // 分钟下拉菜单
    private List<ReminderData> reminderData = new List<ReminderData>();//记事数据列表
    private List<GameObject> reminderobject = new List<GameObject>();//记事物体列表
    private List<GameObject> outreminderobject = new List<GameObject>();//记事物体列表
    private List<int> destroyindex = new List<int>();
    public ScrollRect scrollRect;//滚动条
    private int x;//记事条排列的位置
    private int notificationId = 0;
    public GameObject androidsettingtip;//提示ui

#if UNITY_ANDROID && !UNITY_EDITOR
    void Start()
    {
        // PlayerPrefs.DeleteAll(); // 清除所有 PlayerPrefs 数据，便于测试
        // 为提交按钮添加点击事件
        submitButton.onClick.AddListener(OnSubmit);
        AndroidNotificationCenter.OnNotificationReceived += NotificationReceived;
        LoadReminderData();
        CheckNotification();
    }
    void OnSubmit()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!AndroidNotificationManager.Instance.CheckPermissionStatus())//先检查权限
                return;
        }
        // 获取输入的文本
        string inputText = inputField.text;
        // 检查输入是否为空
        if (!string.IsNullOrEmpty(inputText))
        {
            string selectedHour = hourDropdown.options[hourDropdown.value].text;
            string selectedMinute = minuteDropdown.options[minuteDropdown.value].text;
            DateTime firetime = CalculateTriggerTime(int.Parse(selectedHour), int.Parse(selectedMinute));//计算时间
#if UNITY_ANDROID && !UNITY_EDITOR
            //发送通知，返回通知的id
             notificationId = AndroidNotificationManager.Instance.ScheduleNotification(firetime, inputText);
            if (notificationId == -1)//发送失败
            {
                androidsettingtip.SetActive(true);
                Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
                tiptext.text = firetime.ToString("HH:mm") + "通知发送失败,可能是禁止设备后台启动或者关闭了通知，请按设置获取权限";
                inputField.text = string.Empty;
                return;
            }
#else
            notificationId++;//unity测试id号
#endif
            reminderData.Add(new ReminderData(inputText, firetime, notificationId));
            // 实例化记事条预制体
            GameObject newReminder = Instantiate(ReminderPrefab, ReminderList);
            GameObject newOutReminder = Instantiate(OutReminderPrefab, OutReminderList);
            Reminder reminderComponent = newReminder.GetComponent<Reminder>();
            Reminder outreminderComponent = newOutReminder.GetComponent<Reminder>();
            // GameObject保存到列表
            reminderobject.Add(newReminder);
            outreminderobject.Add(newOutReminder);
            reminderData.Sort((a, b) => a.firetime.CompareTo(b.firetime));//排序

            SaveReminderData();
            for (int j = 0; j < reminderData.Count; j++)
            {
                // 获取对应的 GameObject
                GameObject reminderFromDict = reminderobject[j];
                GameObject outReminderFromDict = outreminderobject[j];
                //单个预制体数据
                Reminder reminderComponentFromDict = reminderFromDict.GetComponent<Reminder>();
                Reminder outreminderComponentFromDict = outReminderFromDict.GetComponent<Reminder>();
                reminderComponentFromDict.reminderData = reminderData[j];
                outreminderComponentFromDict.reminderData = reminderData[j];
                // 更新文本内容
                reminderComponentFromDict.reminderText.text = reminderData[j].text;
                outreminderComponentFromDict.reminderText.text = reminderData[j].text;
                reminderComponentFromDict.TimereminderText.text = reminderData[j].firetime.ToString("HH:mm");
                outreminderComponentFromDict.TimereminderText.text = reminderData[j].firetime.ToString("HH:mm");
                reminderComponentFromDict.TimeMDreminderText.text = reminderData[j].firetime.ToString("MM/dd");
                outreminderComponentFromDict.TimeMDreminderText.text = reminderData[j].firetime.ToString("MM/dd");
                if (reminderData[j].ID == notificationId)//如果新加的ID等于列表中的ID，找到新加数据的位置
                {
                    x = j + 1;

                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ReminderList.gameObject.GetComponent<RectTransform>());//刷新UI列表
            ScrollToIndex(x);
            ReminderColor(x - 1);//闪烁颜色
            // 清空输入框
            inputField.text = string.Empty;
            outreminderComponent.DestroyButton.onClick.AddListener(() => DestroyReminder(newReminder, newOutReminder, reminderComponent));
            outreminderComponent.OutReminderButton.onClick.AddListener(() =>
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    GameObject reminderPanel = canvas.transform.Find("ReminderPanel").gameObject;
                    PanelDOTween.Instance.OnPanelUp(reminderPanel.GetComponent<RectTransform>());
                });
        }
    }
    void SaveReminderData()//保存数据
    {
        // 序列化 reminderData 为 JSON 字符串
        string json = JsonUtility.ToJson(new ReminderDataList(reminderData));
        // 保存到 PlayerPrefs
        PlayerPrefs.SetString("ReminderData", json);
        PlayerPrefs.Save();
    }
    void LoadReminderData()//加载数据
    {
        if (PlayerPrefs.HasKey("ReminderData"))
        {
            string json = PlayerPrefs.GetString("ReminderData");
            ReminderDataList loadedData = JsonUtility.FromJson<ReminderDataList>(json);
            reminderData = loadedData.reminderData;
            // 更新 UI
            UpdateReminderUI();
        }

    }
    void UpdateReminderUI()
    {

        foreach (var data in reminderData)
        {
            if (data == null)
            {
                Debug.LogWarning("ReminderData 为 null，跳过该项");
                continue;
            }
            data.ParseFiretime();
            // 重新生成提醒条目
            GameObject newReminder = Instantiate(ReminderPrefab, ReminderList);
            GameObject newOutReminder = Instantiate(OutReminderPrefab, OutReminderList);
            Reminder reminderComponent = newReminder.GetComponent<Reminder>();
            Reminder outreminderComponent = newOutReminder.GetComponent<Reminder>();
            reminderComponent.reminderText.text = data.text;
            reminderComponent.TimereminderText.text = data.firetime.ToString("HH:mm");
            reminderComponent.TimeMDreminderText.text = data.firetime.ToString("MM/dd");
            outreminderComponent.reminderText.text = data.text;
            outreminderComponent.TimereminderText.text = data.firetime.ToString("HH:mm");
            outreminderComponent.TimeMDreminderText.text = data.firetime.ToString("MM/dd");
            reminderComponent.reminderData = data;
            outreminderComponent.reminderData = data;
            // 将新的提醒条目添加到列表中
            reminderobject.Add(newReminder);
            outreminderobject.Add(newOutReminder);
            outreminderComponent.DestroyButton.onClick.AddListener(() => DestroyReminder(newReminder, newOutReminder, reminderComponent));
            outreminderComponent.OutReminderButton.onClick.AddListener(() =>
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    GameObject reminderPanel = canvas.transform.Find("ReminderPanel").gameObject;
                    PanelDOTween panelDOTween = newOutReminder.GetComponent<PanelDOTween>();
                    panelDOTween.OnPanelUp(reminderPanel.GetComponent<RectTransform>());
                    SaveReminderData();
                });
        }
    }

    void DestroyReminder(GameObject newReminder, GameObject newOutReminder, Reminder reminderComponent)//删除记事条
    {
        Destroy(newReminder);
        Destroy(newOutReminder);
        // 从列表移除GameObject
        reminderobject.Remove(newReminder);
        outreminderobject.Remove(newOutReminder);
        for (int j = 0; j < reminderData.Count; j++)//遍历整个列表，找到要删除的id。
        {
            if (reminderData[j].ID == reminderComponent.reminderData.ID)

            {
                AndroidNotificationManager.Instance.CancelNotification(reminderData[j].ID);//取消通知
                reminderData.RemoveAt(j);
                break;
            }
        }
        SaveReminderData();
    }
    void NotificationReceived(AndroidNotificationIntentData data)
    {
        for (int i = 0; i < reminderobject.Count; i++)//删除记事条
        {
            Reminder reminderComponent = reminderobject[i].GetComponent<Reminder>();//找到记事条的reminder数据
            if (data.Id == reminderComponent.reminderData.ID)//记事条reminder的id数据和要删的reminderdata数据一致
            {
                DestroyReminder(reminderobject[i], outreminderobject[i], reminderComponent);
            }
        }
    }
    void CheckNotification()//检查通知是否发达
    {
        DateTime now = DateTime.Now;
        for (int i = 0; i < reminderData.Count; i++)
        {
            if (reminderData[i].firetime <= now)//判断列表中的时间是否比现在的时间早
            {
                destroyindex.Add(i);//比现在早的通知添加进销毁列表
                string state = AndroidNotificationManager.Instance.CheckNotificationStatus(reminderData[i].ID);
                if (state == "Delivered")
                {
                    if (!AndroidNotificationManager.Instance.CheckPermissionStatustostring())
                    {
                        androidsettingtip.SetActive(true);
                        Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
                        tiptext.text = reminderData[i].firetime.ToString("HH:mm") + "前多条通知未能在准确的时间送达,可能是禁止设备后台启动或者关闭了通知，请按设置获取权限";
                    }

                }
                if (state == "Unknown")
                {
                    androidsettingtip.SetActive(true);
                    Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
                    tiptext.text = reminderData[i].firetime.ToString("HH:mm") + "前多条通知未能在准确的时间送达,可能是禁止设备后台启动或者关闭了通知，请按设置获取权限"; ;
                }
                if (state == "Unavailable")
                {
                    androidsettingtip.SetActive(true);
                    Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
                    tiptext.text = reminderData[i].firetime.ToString("HH:mm") + "前多条通知未能在准确的时间送达,可能是禁止设备后台启动或者关闭了通知，请按设置获取权限"; ;
                }

            }

        }
        for (int i = destroyindex.Count - 1; i > -1; i--)//执行销毁
        {
            int index = destroyindex[i];
            Reminder reminderComponent = reminderobject[index].GetComponent<Reminder>();//找到记事条的reminder数据
            DestroyReminder(reminderobject[index], outreminderobject[index], reminderComponent);
        }
    }


    // 计算触发时间（处理跨天情况）
    private DateTime CalculateTriggerTime(int hour, int minute)
    {
        DateTime now = DateTime.Now;
        DateTime triggerTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        // 如果时间已过，设置为明天
        if (triggerTime <= now)
        {
            triggerTime = triggerTime.AddDays(1);
        }

        return triggerTime;
    }
    void ScrollToIndex(int index)
    {

        float p = Mathf.Max(0.001f, 1.0f - (float)index / reminderData.Count);
        if (p > 0.9f)
            p = 1f;
        Debug.Log(p);
        scrollRect.verticalNormalizedPosition = p;

    }
    void ReminderColor(int index)
    {
        Image reminderimage = reminderobject[index].GetComponentInChildren<Image>();
        PanelDOTween.Instance.OnImageColor(reminderimage, Color.yellow, 0.5f);
    }
#endif
}

[Serializable]
public class ReminderDataList
{
    public List<ReminderData> reminderData;

    public ReminderDataList(List<ReminderData> data)
    {
        reminderData = data;
    }
}