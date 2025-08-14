using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;  // 静态实例

    public Text scoreText;  // 用于显示分数的 Text 组件
    private int score;      // 当前分数


    void Awake()
    {
        // 检查是否已经有实例存在
        if (Instance == null)
        {
            Instance = this;  // 如果没有实例，当前对象就是实例
            DontDestroyOnLoad(gameObject);  // 保证这个对象在场景切换时不销毁
        }
        else if (Instance != this)
        {
            Destroy(gameObject);  // 如果已经有实例，销毁当前对象，确保只有一个 ScoreManager 实例
        }
    }

    // Start method
    void Start()
    {
        score = 0; // 初始分数为 0
        UpdateScoreText();  // 初次显示分数
    }

    // Update is called once per frame
    void Update()
    {
        // 如果你有其他的实时更新逻辑，可以在这里添加
    }

    // 增加分数
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    // 减少分数
    public void SubtractScore(int amount)
    {
        score -= amount;
        UpdateScoreText();
    }

    // 更新分数显示
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString(); // 更新 Text 组件显示分数
        AdjustScoreTextWidth();
    }
    private void AdjustScoreTextWidth()
    {
        // 计算分数的宽度，根据文本的长度来调整
        RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
        float textWidth = scoreText.preferredWidth; // 获取文本的推荐宽度
        scoreRect.sizeDelta = new Vector2(textWidth, scoreRect.sizeDelta.y); // 调整宽度
    }


}