using UnityEngine;
using UnityEngine.UI;  // 引入 UI 命名空间，以便使用 Text 组件
using System; // 引入 DateTime 类

public class DateManager : MonoBehaviour
{
    public Text dateText; // 引用Text 组件

    void Start()
    {
        // 获取当前日期并显示
        UpdateDate();
    }

    void UpdateDate()
    {
        // 获取当前日期
        DateTime currentDate = DateTime.Now;
        // 设置当前文化为英语（美国），确保日期格式为英文
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        // 格式化日期为 "Tue, Jul 1"
        string formattedDate = currentDate.ToString("ddd, MMM d");

        // 显示日期
        if (dateText != null)
        {
            dateText.text = formattedDate;
        }
        else
        {
            Debug.LogError("Date Text component is not assigned.");
        }
    }
}