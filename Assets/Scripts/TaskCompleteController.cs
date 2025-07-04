using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskCompleteController : MonoBehaviour
{
    public Text taskCompleteText; // 显示完成任务的文本
    public float displayTime = 2f; // 显示任务完成控件的持续时间
    public float animationTime = 2f; // 控件从屏幕上方弹出的速度

    private RectTransform rectTransform;
    public GameObject TaskComplete;        // 需要弹出的面板

    private int completedTasks = 0; // 已完成任务数
    private int totalTasks; // 总任务数，可以从 TaskManager 获取
    private Vector2 startPos;
    void Start()
    {
        // 获取 RectTransform
        rectTransform = TaskComplete.GetComponent<RectTransform>();
        TaskComplete.SetActive(false);//开始隐藏面板


        startPos = rectTransform.anchoredPosition;// 获取面板的起始位置

        // 订阅 TaskManager 事件，更新已完成任务的数量
        TaskManager.OnTaskCompleted += UpdateCompletedTasks; // 将更新完成任务方法添加进事件中
    }



    // 显示任务完成进度
    public void ShowTaskComplete()
    {
        taskCompleteText.text = $"{completedTasks}/{totalTasks} daily task completed";
        StartCoroutine(SlideInTaskComplete());//调用协程
    }

    // 更新已完成的任务数
    void UpdateCompletedTasks(int completed, int totalTasks)
    {
        StopAllCoroutines();
        this.totalTasks = totalTasks;
        completedTasks = completed;
        TaskComplete.SetActive(true);
        // StartCoroutine(DisplayAndHideTaskComplete());
        ShowTaskComplete();
    }
    IEnumerator DisplayAndHideTaskComplete()//显示以及隐藏面板
    {



        yield return new WaitForSeconds(displayTime);
        Vector2 endPos = new Vector2(startPos.x, 80f); // 将目标位置设为屏幕外的顶部

        // 动画：从当前位置缓慢滑动到屏幕外
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / animationTime); // 从当前位置滑动到屏幕外
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保面板最终位置准确
        rectTransform.anchoredPosition = endPos;
        TaskComplete.SetActive(false);  // 隐藏面板
        StopAllCoroutines();

    }

    IEnumerator SlideInTaskComplete()
    {


        Vector2 endPos = startPos;

        // 设置初始位置为屏幕上方
        rectTransform.anchoredPosition = new Vector2(startPos.x, 80f);//x不变，y变成屏幕的高度，使得面板往上移动一个屏幕的高度

        // 动画：从屏幕外缓慢滑动到目标位置
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(startPos.x, 80f), endPos, elapsedTime / animationTime);//从屏幕外的位置往最终位置移动
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保面板最终位置准确
        rectTransform.anchoredPosition = endPos;
        StartCoroutine(DisplayAndHideTaskComplete());//调用协程

    }
}