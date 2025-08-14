
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

        LLMService.Instance.OnMicVolumeChanged += volume =>
        {
            Debug.Log("volume: " + volume);
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            LLMService.Instance.SendText("我很高兴你记得我");
    }
}