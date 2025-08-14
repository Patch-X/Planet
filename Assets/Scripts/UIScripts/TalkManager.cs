using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Android;
public class TalkManager : MonoBehaviour
{
    public InputField inputField; // 关联的InputField组件
    public ScrollRect scrollRect;
    public GameObject LeftTalk;
    public GameObject RightTalk;
    public Transform content;
    public float maxWidth = 400f; // 最大宽度
    public float Padding = 20f;//image加上字符左右边距的大小
    public float Paddingedge = 30f;//image距离左右的距离
    public float Paddingedger = 100f;//image距离左右的距离
    string respond = null;
    public ScrollRect wavescrollRect;
    public float scrollSpeed = 0.2f; // 每秒滚动速度
    public GameObject OpenVoiceButton;
    public GameObject CloseVoiceButton;
    public GameObject tip;
    public GameObject androidsettingtip;
    private int androidversion;
    public Image Wave;
    float height = 20f;
    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");//判断安卓版本,Build 内部的 VERSION 类。
        androidversion = buildVersionClass.GetStatic<int>("SDK_INT");//获取安卓版本SDK
#endif
    }
    IEnumerator Start()
    {
        // 等待直到 Manager 初始化完毕
        yield return new WaitUntil(() => LLMService.Instance != null);

        OnRespond();
        // LLMService.Instance.StartVoiceInput();
        LLMService.Instance.OnMicVolumeChanged += volume =>
        {
            height = Mathf.Lerp(20f, 1f, volume);
        };
    }

    void Update()
    {

        if (height < 18f)
        {


            Material mat = Wave.material;
            height = Mathf.Lerp(20f, 1f, height);

            mat.SetFloat("_Height", height);

            // 自动向右滚动
            wavescrollRect.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime;
            // 到达最右侧后回到最左侧，实现循环
            if (wavescrollRect.horizontalNormalizedPosition > 1f)
                wavescrollRect.horizontalNormalizedPosition = 0f;
        }
        else
        {
            Material mat = Wave.material;
            mat.SetFloat("_Height", height);
        }
    }

    public void OnInput()
    {
        // 获取输入的文本
        string inputText = inputField.text;
        if (!string.IsNullOrEmpty(inputText))
        {
            GameObject newRightTalk = Instantiate(RightTalk, content);//实例化对象
            Text rightText = newRightTalk.GetComponentInChildren<Text>();
            rightText.text = inputText;
            float width = Mathf.Min(rightText.preferredWidth + Padding, maxWidth + Padding);//计算文本宽度，取最小值
            Image rightImage = newRightTalk.GetComponentInChildren<Image>();
            RectTransform rt = rightImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
            VerticalLayoutGroup layout = newRightTalk.GetComponent<VerticalLayoutGroup>();
            float PaddingedgeRight = 720f - width - Paddingedge;//右边距

            layout.padding.left = (int)PaddingedgeRight;//左气泡往左移动距离
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);//刷新刷新 ContentSizeFitter
            LayoutRebuilder.ForceRebuildLayoutImmediate(newRightTalk.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            StartCoroutine(RefreshAndScroll());
            // 滚动到最底部
            LLMService.Instance.SendText(inputText);
            // 清空输入框
            inputField.text = string.Empty;

        }
    }

    public void OnRespond()
    {
        LLMService.Instance.OnMessageReceived += msg =>
        {
            MsgData data = JsonUtility.FromJson<MsgData>(msg);
            if (!string.IsNullOrEmpty(data.text) && data.type != "stt" && data.state != "sentence_end")
                respond = data.text;
            if (!string.IsNullOrEmpty(data.emotion))
                respond += $" ({data.emotion})";
            if (!string.IsNullOrEmpty(respond))
            {
                GameObject newLeftTalk = Instantiate(LeftTalk, content);
                Text leftText = newLeftTalk.GetComponentInChildren<Text>();
                leftText.text = respond;
                float width = Mathf.Min(leftText.preferredWidth + Padding, maxWidth + Padding);
                Image leftImage = newLeftTalk.GetComponentInChildren<Image>();
                RectTransform rt = leftImage.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
                VerticalLayoutGroup layout = newLeftTalk.GetComponent<VerticalLayoutGroup>();
                layout.padding.left = (int)Paddingedge;//左气泡往左移动距离
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                LayoutRebuilder.ForceRebuildLayoutImmediate(newLeftTalk.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                StartCoroutine(RefreshAndScroll()); // 滚动到最底部
                respond = null;
            }
        };


    }
    IEnumerator RefreshAndScroll()
    {
        yield return null; // 等待一帧
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0;
    }
    private void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)//当用户选择“拒绝且不再询问”权限请求
    {
        androidsettingtip.SetActive(true);
        Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
        tiptext.text = "录音权限被拒绝,如需要使用此功能请按设置->权限->麦克风->允许";
    }
    private void PermissionCallbacks_PermissionGranted(string permissionName)//当用户选择允许权限请求时触发。
    {
        LLMService.Instance.StartVoiceInput();//开启录音
        // 已授权，隐藏Open，显示Close
        OpenVoiceButton.SetActive(false);
        CloseVoiceButton.SetActive(true);
    }
    private void PermissionCallbacks_PermissionDenied(string permissionName)//当用户选择拒绝权限请求时触发。
    {
        if (androidversion < 31)
        {
            tip.SetActive(true);
            Text tiptext = tip.GetComponentInChildren<Text>();
            tiptext.text = "录音权限被拒绝,如需要使用此功能请打开权限";
        }
        else
        {
            androidsettingtip.SetActive(true);
            Text tiptext = androidsettingtip.GetComponentInChildren<Text>();
            tiptext.text = "录音权限被拒绝,如需要使用此功能请按设置->权限->麦克风->允许";
        }

    }
    public void OnOpenVoiceButton()
    {
        Debug.Log("androidversion:" + androidversion);
        androidsettingtip.SetActive(false);
        tip.SetActive(false);
        //检查是否授权
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            LLMService.Instance.StartVoiceInput();//开启录音
            // 已授权，隐藏Open，显示Close
            OpenVoiceButton.SetActive(false);
            CloseVoiceButton.SetActive(true);
        }
        else
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;//当用户选择拒绝权限请求时触发。
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;//当用户选择允许权限请求时触发。
            if (androidversion < 31)//安卓12以下
            {
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;//当用户选择“拒绝且不再询问”权限请求
            }
            Permission.RequestUserPermission(Permission.Microphone, callbacks);//请求授权并且发送回调


        }
    }
    public void OnCloseVoiceButton()
    {
        LLMService.Instance.StopVoiceInput();//关闭录音
        OpenVoiceButton.SetActive(true);
        CloseVoiceButton.SetActive(false);
        height = 20f;
    }


}
[Serializable]
public class AudioParams
{
    public string format;
    public int sample_rate;
    public int channels;
    public int frame_duration;
}

[Serializable]
public class MsgData
{
    public string state;//消息状态
    public string type;//消息类型
    public string text;//文本
    public string emotion;//表情
    public AudioParams audio_params;
}
