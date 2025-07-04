using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private int triangleCount = 0;
    private string triangleText = "";

    void Start()
    {
        Application.targetFrameRate = 1000; // 或 60、120 等你想测试的上限
        QualitySettings.vSyncCount = 0;    // 禁用垂直同步，否则会影响帧率上限

        CountTriangles();
    }

    void Update()
    {
        // FPS 平滑计算
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void CountTriangles()
    {
        int totalTriangles = 0;
        MeshFilter[] meshes = FindObjectsOfType<MeshFilter>();

        foreach (MeshFilter mf in meshes)
        {
            if (mf.sharedMesh != null)
                totalTriangles += mf.sharedMesh.triangles.Length / 3;
        }

        triangleCount = totalTriangles;
        triangleText = $"Triangles: {triangleCount}";
        Debug.Log(triangleText);
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 100, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h / 50;
        style.normal.textColor = Color.white;

        float fps = 1.0f / deltaTime;
        string text = $"FPS: {fps:0.}   |   {triangleText}";
        GUI.Label(rect, text, style);
    }
}