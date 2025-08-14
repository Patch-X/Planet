using UnityEngine;
////using NativeWebSocket;
using Concentus.Structs;
using Concentus.Enums;
using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;

public class ConcentusVoiceClient : MonoBehaviour
{
    [Header("WebSocket 配置")]
    public string websocketUrl = "ws://106.53.22.125:8000/xiaozhi/v1/";
    public string bearerToken = "<your-token>";

    public event Action<string> OnMessageReceived;
    public event Action<float> OnMicVolumeChanged;
    public event Action OnTTSStarted;
    public event Action OnTTSEnded;

    //private WebSocket socket;
    private AudioClip micClip;
    private int micPosition = 0;
    private const int sampleRate = 16000;
    private const int frameSize = 960;
    private float[] pcmBuffer = new float[frameSize];
    private short[] pcmShorts = new short[frameSize];
    private byte[] opusBuffer = new byte[512];
    private OpusEncoder encoder;
    private OpusDecoder decoder;
    private AudioSource audioSource;
    private const int playbackChannels = 1;

    private bool readyToSend = false;

    async void Start()
    {
        encoder = new OpusEncoder(sampleRate, 1, OpusApplication.OPUS_APPLICATION_AUDIO);
        encoder.Bitrate = 24000;

        decoder = new OpusDecoder(sampleRate, playbackChannels);
        audioSource = gameObject.AddComponent<AudioSource>();

        //socket = new WebSocket(websocketUrl);
        //socket.SetHeader("Authorization", "Bearer " + bearerToken);
        //socket.SetHeader("Protocol-Version", "1");
        //// socket.SetHeader("Device-Id", "AF:01:41:0B:C8:28");
        //socket.SetHeader("Device-Id", "01:23:45:67:89:AB");
        //socket.SetHeader("Client-Id", SystemInfo.deviceUniqueIdentifier);

        //socket.OnOpen += () => SendHello();
        //socket.OnClose += code => Debug.LogWarning("Socket closed: " + code);
        //socket.OnError += err => Debug.LogError("Socket error: " + err);

        //socket.OnMessage += (bytes) =>
        //{
        //    string msgStr = null;
        //    try { msgStr = Encoding.UTF8.GetString(bytes); } catch { }

        //    if (!string.IsNullOrEmpty(msgStr) && msgStr.Contains("type"))
        //    {
        //        // Debug.Log("📥 JSON Msg: " + msgStr);
        //        // if (msgStr.Contains("hello")) SendListenStart();
        //        OnMessageReceived?.Invoke(msgStr);
        //    }
        //    else
        //    {
        //        OnTTSStarted?.Invoke();
        //        HandleAudioResponse(bytes);
        //    }
        //};

        //await socket.Connect();
    }

    async void SendHello()
    {
        Debug.Log("SendHello");
        var hello = new
        {
            type = "hello",
            version = 1,
            transport = "websocket",
            audio_params = new
            {
                format = "opus",
                sample_rate = 16000,
                channels = 1,
                frame_duration = 60
            }
        };
        //await socket.SendText(JsonConvert.SerializeObject(hello));
    }

    async void SendListenStart()
    {
        var listen = new
        {
            session_id = "",
            type = "listen",
            state = "start",
            mode = "manual"
        };
        //await socket.SendText(JsonConvert.SerializeObject(listen));

        StartMicrophone();
        readyToSend = true;
        StartCoroutine(SendAudioLoop());
    }

    public void StartVoiceInteraction() => SendListenStart();

    public void StopVoiceInteraction()
    {
        readyToSend = false;
        StopCoroutine(SendAudioLoop());
        Microphone.End(null);
    }

    void StartMicrophone()
    {
        micClip = Microphone.Start(null, true, 1, sampleRate);
    }

    IEnumerator SendAudioLoop()
    {
        while (readyToSend)
        {
            int position = Microphone.GetPosition(null);
            if (position < micPosition) micPosition = 0;

            if (position - micPosition >= frameSize)
            {
                micClip.GetData(pcmBuffer, micPosition);
                micPosition += frameSize;

                for (int i = 0; i < frameSize; i++)
                    pcmShorts[i] = (short)(Mathf.Clamp(pcmBuffer[i], -1f, 1f) * short.MaxValue);

                int len = encoder.Encode(pcmShorts, 0, frameSize, opusBuffer, 0, opusBuffer.Length);
                byte[] frame = new byte[len];
                Array.Copy(opusBuffer, frame, len);

                //socket.Send(frame);
            }

            float max = 0f;

            for (int i = 0; i < frameSize; i++)
            {
                float clamped = Mathf.Clamp(pcmBuffer[i], -1f, 1f);
                pcmShorts[i] = (short)(clamped * short.MaxValue);
                max = Mathf.Max(max, Mathf.Abs(clamped));  // 取当前帧的最大振幅
            }

            // 通知 UI 层展示音量波动（范围在 0~1 之间）
            OnMicVolumeChanged?.Invoke(max);

            yield return new WaitForSeconds(0.06f);
        }
    }

    void HandleAudioResponse(byte[] opusData)
    {
        short[] pcm = new short[frameSize * playbackChannels];
        int decodedSamples = decoder.Decode(opusData, 0, opusData.Length, pcm, 0, frameSize, false);

        if (decodedSamples > 0)
        {
            float[] samples = new float[decodedSamples];
            for (int i = 0; i < decodedSamples; i++)
                samples[i] = pcm[i] / 32768f;

            PlayAudioClip(samples);
        }
    }

    void PlayAudioClip(float[] samples)
    {
        AudioClip clip = AudioClip.Create("TTS", samples.Length, playbackChannels, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.PlayOneShot(clip);
        StartCoroutine(NotifyTTSEnd(clip.length));
    }

    IEnumerator NotifyTTSEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnTTSEnded?.Invoke();
    }

    public async void SendTextMessage(string text)
    {
        //if (string.IsNullOrWhiteSpace(text) || socket == null || socket.State != WebSocketState.Open) return;

        //var msg = new
        //{
        //    type = "listen",
        //    mode = "manual",
        //    state = "detect",
        //    text = text.Trim()
        //};
        //await socket.SendText(JsonConvert.SerializeObject(msg));
    }

    //void Update()
    //{
    //    socket?.DispatchMessageQueue();
    //}

    //async void OnDestroy()
    //{
    //    readyToSend = false;
    //    if (socket != null && socket.State == WebSocketState.Open)
    //        await socket.Close();
    //}
}
