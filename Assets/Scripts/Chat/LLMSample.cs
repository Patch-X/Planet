
using UnityEngine;
using System.Collections;

public class LLMSample : MonoBehaviour
{
    IEnumerator Start()
    {
        // 等待直到 Manager 初始化完毕
        yield return new WaitUntil(() => LLMService.Instance != null);

        SetRespond();
        LLMService.Instance.StartVoiceInput();
    }

    void SetRespond()
    {
        LLMService.Instance.OnMessageReceived += msg =>
        {
            Debug.Log("🧠 AI Response: " + msg);
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            LLMService.Instance.SendText("你好，今天天气如何？");
    }
}