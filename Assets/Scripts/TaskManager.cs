using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class TaskManager : MonoBehaviour
{
    public GameObject TaskPrefab;          // 任务预制体
    public Transform TaskListParent;       // 任务列表父物体
    public List<TaskData> taskData;//任务数据
    private RectTransform TaskListParentRectTransform;//获取任务列表的RectTransform
    public static event Action<int, int> OnTaskCompleted; // 任务完成时触发的事件

    private int completedTasks;
    private int totalTask;

    private void Start()
    {
        TaskListParentRectTransform = TaskListParent.GetComponent<RectTransform>();
        // 自动生成若干个任务
        for (int i = 0; i < taskData.Count; i++) // 根据文本列表生成任务
        {
            GenerateTask(i);
            Vector2 size = TaskListParentRectTransform.sizeDelta;//每次生成任务，动态生成任务列表长度
            size.y += 148f;
            TaskListParentRectTransform.sizeDelta = size;

        }
        totalTask = taskData.Count;
    }

    // 生成一个任务
    void GenerateTask(int i)
    {

        // 实例化一个 Task 预制体
        GameObject newTask = Instantiate(TaskPrefab, TaskListParent);
        GameObject TaskButton = newTask.transform.GetChild(0).gameObject;
        // 获取各个组件
        Image icon = TaskButton.transform.GetChild(1).GetComponent<Image>();  // 假设图标在子物体中
        Text taskText = TaskButton.transform.GetChild(0).GetComponent<Text>();   // 假设文本在子物体中
        Image isCompleteIcon = TaskButton.transform.GetChild(2).GetComponent<Image>();
        GameObject CompleteButton = TaskButton.transform.GetChild(3).gameObject;
        Text score = TaskButton.transform.GetChild(4).GetComponent<Text>();

        if (ColorUtility.TryParseHtmlString(taskData[i].iconColor, out Color color))
        {
            icon.color = color;  // 将解析后的颜色应用到图标
        }
        else
        {
            icon.color = Color.white;  // 如果解析失败，使用默认颜色
        }


        taskText.text = taskData[i].taskText;// 显示文本
        score.text = taskData[i].score.ToString();//显示分数
        // 设置 isCompleteIcon 初始颜色为灰色
        isCompleteIcon.color = Color.gray;

        // 为 Task 设置点击事件，点击后 isCompleteIcon 变成蓝色
        Button taskButton = TaskButton.GetComponent<Button>();
        taskButton.onClick.AddListener(() => OnTaskClicked(i, isCompleteIcon, taskText, taskButton, CompleteButton));
    }

    // 任务点击事件：改变 isCompleteIcon 的颜色，改变文本透明度
    void OnTaskClicked(int i, Image isCompleteIcon, Text taskText, Button taskButton, GameObject CompleteButton)
    {

        isCompleteIcon.color = Color.blue;  // 点击后变为蓝色
        Color textColor = taskText.color;  // 获取当前颜色
        textColor.a = 0.5f;  // 修改 alpha 值
        taskText.color = textColor;  // 赋值回去
        CompleteTask();//调用CompleteTask消息弹出；
        taskButton.interactable = false;
        CompleteButton.SetActive(true);// 显示完成按钮
        CompleteButton.GetComponent<Button>().onClick.AddListener(() =>
    {
        ScoreManager.Instance.AddScore(taskData[i].score); // 增加分数
        CompleteButton.SetActive(false);  // 点击完成按钮后隐藏它
    });

    }
    public void CompleteTask()
    {
        completedTasks++;
        OnTaskCompleted?.Invoke(completedTasks, totalTask); // 通知 TaskCompleteController
    }


}
