using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PanelDOTween : MonoBehaviour
{
    public static PanelDOTween Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float animationTime = 0f;// 动画时间
    public void OnImageColor(Image image, Color endcolor, float t)
    {
        image.DOColor(endcolor, t).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);  // 设置缓动类型为线性
    }
    public void OnStyleButtonClick(RectTransform Scrolls)
    {

        Scrolls.DOLocalMoveX(-Scrolls.rect.width / 2f, animationTime);
    }
    public void OnStyleClick(RectTransform Style)
    {


        Style.DOLocalMoveX(-Style.rect.width / 4f, animationTime);
    }
    public void OnClothesClick(RectTransform Clothes)
    {

        Clothes.DOLocalMoveX(Clothes.rect.width / 4f, animationTime);

    }

    public void OnClothesButtonClick(RectTransform Scrolls)
    {

        Scrolls.DOLocalMoveX(Scrolls.rect.width / 2f, animationTime);

    }
    public void OnPanelDown(RectTransform panel)
    {
        panel.gameObject.SetActive(true);  // 显示面板

        // 获取面板的起始位置
        Vector2 startPos = panel.anchoredPosition;

        // 设置面板的初始位置为屏幕高度以外
        panel.anchoredPosition = new Vector2(startPos.x, Screen.height);  // x不变，y变成屏幕的高度，使得面板往上移动一个屏幕的高度

        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveY(0, animationTime);  // y轴平滑移动到0（即父对象中心点）

    }
    public void OnPanelDownBack(RectTransform panel)
    {
        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveY(Screen.height, animationTime).OnComplete(() => panel.gameObject.SetActive(false));
    }
    public void OnPanelUp(RectTransform panel)
    {
        panel.gameObject.SetActive(true);  // 显示面板

        // 获取面板的起始位置
        Vector2 startPos = panel.anchoredPosition;

        // 设置面板的初始位置为屏幕高度以外
        panel.anchoredPosition = new Vector2(startPos.x, -Screen.height);  // x不变，y变成屏幕的高度，使得面板往上移动一个屏幕的高度

        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveY(0, animationTime);  // y轴平滑移动到0（即父对象中心点）

    }
    public void OnPanelUpBack(RectTransform panel)
    {
        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveY(-Screen.height, animationTime).OnComplete(() => panel.gameObject.SetActive(false));

    }
    public void OnPanelLeft(RectTransform panel)
    {
        panel.gameObject.SetActive(true);  // 显示面板

        // 获取面板的起始位置
        Vector2 startPos = panel.anchoredPosition;

        // 设置面板的初始位置为屏幕高度以外
        panel.anchoredPosition = new Vector2(Screen.height, startPos.y);

        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveX(0, animationTime);  // x轴平滑移动到0（即父对象中心点）

    }
    public void OnPanelRight(RectTransform panel)
    {
        panel.gameObject.SetActive(true);  // 显示面板

        // 获取面板的起始位置
        Vector2 startPos = panel.anchoredPosition;

        // 设置面板的初始位置为屏幕高度以外
        panel.anchoredPosition = new Vector2(-Screen.height, startPos.y);

        // 移动面板到屏幕可见区域的目标位置
        panel.DOLocalMoveX(0, animationTime);  // x轴平滑移动到0（即父对象中心点）

    }
    public void OnPanelLeftBack(RectTransform panel)
    {
        panel.DOLocalMoveX(Screen.height, animationTime).OnComplete(() => panel.gameObject.SetActive(false));
    }

    public void OnPanelFadeIn(RectTransform panel)
    {
        CanvasGroup canvasGroup = panel.gameObject.GetComponent<CanvasGroup>();
        panel.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, animationTime);
    }
    public void OnPanelFadeout(RectTransform panel)
    {
        CanvasGroup canvasGroup = panel.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0f, animationTime).OnComplete(() => panel.gameObject.SetActive(false));
    }
    public void OnSwitchClick(RectTransform Scrolls, bool ismusic)
    {
        Image Area = Scrolls.gameObject.transform.parent.GetComponent<Image>();
        if (!ismusic)
        {
            Area.color = new Color(1, 1, 1, 1);
            Scrolls.DOLocalMoveX(-Scrolls.rect.width / 2f, animationTime);

        }
        else
        {
            Area.color = new Color32(81, 255, 117, 255);
            Scrolls.DOLocalMoveX(Scrolls.rect.width / 2f, animationTime);

        }
    }

}