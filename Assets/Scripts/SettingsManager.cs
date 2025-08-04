using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // 核心数据 编译期
    [HideInInspector] public static int GirdSide = 10;
    [HideInInspector] public static float PlanetRadius = 50.0f;
    [HideInInspector] public static string expandFileName = "CubeSphereGrid.json";

    // 设置项（可扩展）
    public int maxResolutionWidth = 1280;
    public int maxResolutionHeight = 720;
    public float masterVolume = 1f;
    public string language = "en";
    public int graphicsQuality = 2;

    private void Awake()
    {
        // 单例模式 + 防止重复创建
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景保留

        LoadSettings();
    }

    public void LoadSettings()
    {
        // 示例加载
        maxResolutionWidth = PlayerPrefs.GetInt("MaxWidth", 1280);
        maxResolutionHeight = PlayerPrefs.GetInt("MaxHeight", 720);
        masterVolume = PlayerPrefs.GetFloat("Volume", 1f);
        language = PlayerPrefs.GetString("Language", "en");
        graphicsQuality = PlayerPrefs.GetInt("Graphics", 2);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("MaxWidth", maxResolutionWidth);
        PlayerPrefs.SetInt("MaxHeight", maxResolutionHeight);
        PlayerPrefs.SetFloat("Volume", masterVolume);
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.SetInt("Graphics", graphicsQuality);
        PlayerPrefs.Save();
    }
}
