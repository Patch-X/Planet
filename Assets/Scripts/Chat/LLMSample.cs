
using UnityEngine;
using System.Collections;

public class LLMSample : MonoBehaviour
{
    IEnumerator Start()
    {
        // 等待直到 Manager 初始化完毕
        yield return new WaitUntil(() => VoiceService.Instance != null);

        SetRespond();
        VoiceService.Instance.StartVoiceInput();
    }

    void SetRespond()
    {
        VoiceService.Instance.OnMessageReceived += msg =>
        {
            Debug.Log("🧠 AI Response: " + msg);
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            VoiceService.Instance.SendText("你好，今天天气如何？");
    }
}