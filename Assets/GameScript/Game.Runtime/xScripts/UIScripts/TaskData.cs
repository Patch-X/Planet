
[System.Serializable]
public class TaskData
{
    public string taskText;//任务文本
    public int score;//分数
    public string iconColor;//图标颜色
    public TaskData(string taskText, int score, string iconColor)
    {
        this.taskText = taskText;
        this.score = score;
        this.iconColor = iconColor;
    }

}
