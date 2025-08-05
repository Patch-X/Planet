using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.Notifications.Android;

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
    private int i;//列表索引
    private int x;//记事条排列的位置
    public GameObject tip;//提示ui
    void Start()
    {


        // 为提交按钮添加点击事件
        submitButton.onClick.AddListener(OnSubmit);
        AndroidNotificationCenter.OnNotificationReceived += NotificationReceived;
        LoadReminderData();

    }
    // void Update()
    // {
    //     if (Application.platform == RuntimePlatform.Android)
    //     {

    //         if (reminderData != null)//如果记事数据列表不为空
    //         {
    //             foreach (var data in reminderData)
    //             {
    //                 if (data == null)
    //                 {

    //                     continue;
    //                 }

    //                 string isget = AndroidNotificationManager.Instance.CheckNotificationStatus(data.ID);//检查有无送达消息
    //                 if (isget == "Delivered")//已经送达
    //                 {
    //                     for (int i = 0; i < reminderobject.Count; i++)//删除记事条
    //                     {
    //                         Reminder reminderComponent = reminderobject[i].GetComponent<Reminder>();//找到记事条的reminder数据
    //                         Reminder outreminderComponent = outreminderobject[i].GetComponent<Reminder>();
    //                         if (data.ID == reminderComponent.reminderDataId)//记事条reminder的id数据和要删的reminderdata数据一致
    //                         {
    //                             DestroyReminder(reminderobject[i], outreminderobject[i], reminderComponent);
    //                             SaveReminderData();//保存数据
    //                         }
    //                     }

    //                 }
    //                 else if (isget == "Scheduled")//发送中
    //                 {

    //                     continue;//继续循环
    //                 }
    //                 else
    //                 {
    //                     for (int i = 0; i < reminderobject.Count; i++)//删除记事条
    //                     {
    //                         Reminder reminderComponent = reminderobject[i].GetComponent<Reminder>();//找到记事条的reminder数据
    //                         Reminder outreminderComponent = outreminderobject[i].GetComponent<Reminder>();
    //                         if (data.ID == reminderComponent.reminderDataId)//记事条reminder的id数据和要删的reminderdata数据一致
    //                         {
    //                             DestroyReminder(reminderobject[i], outreminderobject[i], reminderComponent);
    //                             SaveReminderData();//保存数据
    //                         }
    //                     }

    //                     tip.SetActive(true);//弹出UI

    //                 }


    //             }
    //         }
    //     }


    // }

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
            //发送通知，返回通知的id
            int notificationId = AndroidNotificationManager.Instance.ScheduleNotification(int.Parse(selectedHour), int.Parse(selectedMinute), inputText);
            reminderData.Add(new ReminderData(inputText, selectedHour, selectedMinute, notificationId));
            i = reminderData.Count - 1; // 设置 i 为新项的索引
                                        // 实例化记事条预制体
            GameObject newReminder = Instantiate(ReminderPrefab, ReminderList);
            GameObject newOutReminder = Instantiate(OutReminderPrefab, OutReminderList);
            //单个预制体数据
            Reminder reminderComponent = newReminder.GetComponent<Reminder>();
            Reminder outreminderComponent = newOutReminder.GetComponent<Reminder>();
            reminderComponent.reminderDataId = reminderData[i].ID;
            outreminderComponent.reminderDataId = reminderData[i].ID;
            // GameObject保存到列表
            reminderobject.Add(newReminder);
            outreminderobject.Add(newOutReminder);
            reminderData.Sort((a, b) => a.time.CompareTo(b.time));//排序
            SaveReminderData();
            for (int i = 0; i < reminderData.Count; i++)
            {
                if (reminderData[i].ID == reminderComponent.reminderDataId)

                {
                    x = i; //记事条在列表排列的位置

                    break;
                }
            }
            for (int j = 0; j < reminderData.Count; j++)
            {
                // 获取对应的 GameObject
                GameObject reminderFromDict = reminderobject[j];
                GameObject outReminderFromDict = outreminderobject[j];

                Reminder reminderComponentSorted = reminderFromDict.GetComponent<Reminder>();
                Reminder outreminderComponentSorted = outReminderFromDict.GetComponent<Reminder>();

                // 更新文本内容
                reminderComponentSorted.reminderText.text = reminderData[j].text;
                outreminderComponentSorted.reminderText.text = reminderData[j].text;
                reminderComponentSorted.TimereminderText.text = reminderData[j].Hour + ":" + reminderData[j].Minute;
                outreminderComponentSorted.TimereminderText.text = reminderData[j].Hour + ":" + reminderData[j].Minute;
                reminderComponent.reminderDataId = reminderData[j].ID;
                outreminderComponent.reminderDataId = reminderData[j].ID;

            }

            // 清空输入框
            inputField.text = string.Empty;                                                                                                                                                                                                                         //获取当前记事条的索引并计算滚动位置
            float newScrollPosition = 1 - (x / (float)reminderData.Count);
            if (newScrollPosition < 0.125f)
            {
                newScrollPosition = 0f;
            }
            if (newScrollPosition > 0.875f)
            {
                newScrollPosition = 1f;
            }
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
        void UpdateReminderUI()
        {

            foreach (var data in reminderData)
            {
                if (data == null)
                {
                    Debug.LogWarning("ReminderData 为 null，跳过该项");
                    continue;
                }
                // 重新生成提醒条目
                GameObject newReminder = Instantiate(ReminderPrefab, ReminderList);
                GameObject newOutReminder = Instantiate(OutReminderPrefab, OutReminderList);
                Reminder reminderComponent = newReminder.GetComponent<Reminder>();
                Reminder outreminderComponent = newOutReminder.GetComponent<Reminder>();
                reminderComponent.reminderText.text = data.text;
                reminderComponent.TimereminderText.text = data.Hour + ":" + data.Minute;
                outreminderComponent.reminderText.text = data.text;
                outreminderComponent.TimereminderText.text = data.Hour + ":" + data.Minute;
                reminderComponent.reminderDataId = data.ID;
                outreminderComponent.reminderDataId = data.ID;

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
            if (reminderData[j].ID == reminderComponent.reminderDataId)

            {
                AndroidNotificationManager.Instance.CancelNotification(reminderComponent.reminderDataId);//取消通知
                reminderData.RemoveAt(j);
                break;
            }
        }
        i = reminderData.Count - 1; // 更新i索引
        SaveReminderData();
    }
    void NotificationReceived(AndroidNotificationIntentData data)
    {
        for (int i = 0; i < reminderobject.Count; i++)//删除记事条
        {
            Reminder reminderComponent = reminderobject[i].GetComponent<Reminder>();//找到记事条的reminder数据
            Reminder outreminderComponent = outreminderobject[i].GetComponent<Reminder>();
            if (data.Id == reminderComponent.reminderDataId)//记事条reminder的id数据和要删的reminderdata数据一致
            {
                DestroyReminder(reminderobject[i], outreminderobject[i], reminderComponent);
                SaveReminderData();//保存数据
            }
        }
    }
}

[System.Serializable]
public class ReminderDataList
{
    public List<ReminderData> reminderData;

    public ReminderDataList(List<ReminderData> data)
    {
        reminderData = data;
    }
}