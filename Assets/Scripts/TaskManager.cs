using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class TaskManager : MonoBehaviour
{
    public GameObject TaskPrefab;          // 任务预制体
    public Transform TaskListParent;       // 任务列表父物体
    public List<string> randomTexts;       // 随机文本列表
    private RectTransform TaskListParentRectTransform;//获取任务列表的RectTransform
    public static event Action<int, int> OnTaskCompleted; // 任务完成时触发的事件

    private int completedTasks;
    private int totalTask;

    private void Start()
    {
        TaskListParentRectTransform = TaskListParent.GetComponent<RectTransform>();
        // 自动生成若干个任务
        for (int i = 0; i < randomTexts.Count; i++) // 根据文本列表生成任务
        {
            GenerateTask(i);
            Vector2 size = TaskListParentRectTransform.sizeDelta;//每次生成任务，动态生成任务列表长度
            size.y += 148f;
            TaskListParentRectTransform.sizeDelta = size;

        }
        totalTask = randomTexts.Count;
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


        // 随机设置 Icon 颜色
        icon.color = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);  // 随机颜色

        // 随机设置文本
        taskText.text = randomTexts[i];

        // 设置 isCompleteIcon 初始颜色为灰色
        isCompleteIcon.color = Color.gray;

        // 为 Task 设置点击事件，点击后 isCompleteIcon 变成蓝色
        Button taskButton = TaskButton.GetComponent<Button>();
        taskButton.onClick.AddListener(() => OnTaskClicked(isCompleteIcon, taskText, taskButton));
    }

    // 任务点击事件：改变 isCompleteIcon 的颜色，改变文本透明度
    void OnTaskClicked(Image isCompleteIcon, Text taskText, Button taskButton)
    {

        isCompleteIcon.color = Color.blue;  // 点击后变为蓝色
        Color textColor = taskText.color;  // 获取当前颜色
        textColor.a = 0.5f;  // 修改 alpha 值
        taskText.color = textColor;  // 赋值回去
        CompleteTask();
        taskButton.interactable = false;
    }
    public void CompleteTask()
    {
        completedTasks++;
        OnTaskCompleted?.Invoke(completedTasks, totalTask); // 通知 TaskCompleteController
    }
}
