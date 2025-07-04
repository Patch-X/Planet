using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelController : MonoBehaviour
{
    public Button TaskButton;  // 按钮，点击时触发面板弹出
    public Button BackButton; //按钮，点击时触发面板返回
    public ScrollRect TaskScrollRect;  // 绑定 ScrollRect 组件
    public GameObject Taskpanel;        // 需要弹出的面板
    public float animationTime; // 动画时间，控制面板从上往下的动画速度

    private RectTransform panelRectTransform;

    void Start()
    {
        panelRectTransform = Taskpanel.GetComponent<RectTransform>();
        Taskpanel.SetActive(false); // 初始时隐藏面板

        // 给按钮添加点击事件
        TaskButton.onClick.AddListener(OnTask);
        BackButton.onClick.AddListener(OnBack);
    }

    void OnTask()
    {
        if (!Taskpanel.activeSelf)
        {
            StopAllCoroutines();//停止所有协程
            Taskpanel.SetActive(true);  // 显示面板
            TaskScrollRect.verticalNormalizedPosition = 1;//让ScrollRect滚动到顶部
            StartCoroutine(SlideInPanel());

        }
    }
    void OnBack()
    {
        if (Taskpanel.activeSelf)
        {
            StopAllCoroutines(); // 停止所有协程
            StartCoroutine(SlideOutPanel());  // 启动面板退出的协程
        }
    }

    IEnumerator SlideInPanel()
    {

        // 获取面板的起始位置和目标位置
        Vector2 startPos = panelRectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, 0); // 假设面板弹出时 Y 坐标为 0（底部）

        // 设置初始位置为屏幕上方
        panelRectTransform.anchoredPosition = new Vector2(startPos.x, Screen.height);//x不变，y变成屏幕的高度，使得面板往上移动一个屏幕的高度

        // 动画：从屏幕外缓慢滑动到目标位置
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(new Vector2(startPos.x, Screen.height), endPos, elapsedTime / animationTime);//从屏幕外的位置往最终位置移动
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保面板最终位置准确
        panelRectTransform.anchoredPosition = endPos;

    }
    IEnumerator SlideOutPanel()
    {
        // 获取面板的起始位置和目标位置
        Vector2 startPos = panelRectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, Screen.height); // 将目标位置设为屏幕外的顶部

        // 动画：从当前位置缓慢滑动到屏幕外
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / animationTime); // 从当前位置滑动到屏幕外
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保面板最终位置准确
        panelRectTransform.anchoredPosition = endPos;
        Taskpanel.SetActive(false);  // 隐藏面板
    }

}