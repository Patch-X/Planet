using System;
using UnityEngine;

public class VoiceService : MonoBehaviour
{
    public static VoiceService Instance { get; private set; }

    public event Action<string> OnMessageReceived;  // 文本回调
    public event Action OnTTSStarted;               // TTS开始
    public event Action OnTTSEnded;                 // TTS播放结束

    private ConcentusVoiceClient voiceClient;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        voiceClient = gameObject.AddComponent<ConcentusVoiceClient>();
        voiceClient.OnMessageReceived += msg => OnMessageReceived?.Invoke(msg);
        voiceClient.OnTTSStarted += () => OnTTSStarted?.Invoke();
        voiceClient.OnTTSEnded += () => OnTTSEnded?.Invoke();
    }

    public void SendText(string text)
    {
        voiceClient.SendTextMessage(text);
    }

    public void StartVoiceInput()
    {
        voiceClient.StartVoiceInteraction();
    }

    public void StopVoiceInput()
    {
        voiceClient.StopVoiceInteraction();
    }
}
