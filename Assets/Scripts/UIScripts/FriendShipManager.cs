using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class FriendShipManager : MonoBehaviour
{
    public Text alldate;
    public Text finishdate;
    public Text startdate;
    public Slider friendshipslider;
    private FriendShipData friendShipData;
    public List<Image> imagelist;
    private const string LastSignInDateKey = "LastSignInDate";
    private const string AllDaysKey = "TotalDays";
    private const string StartDaysKey = "StartDays";
    private const string FinishDaysKey = "FinishDays";


    void Start()
    {
        //初始化数据
        PlayerPrefs.DeleteKey(LastSignInDateKey);
        PlayerPrefs.DeleteKey(AllDaysKey);
        PlayerPrefs.DeleteKey(StartDaysKey);
        PlayerPrefs.DeleteKey(FinishDaysKey);
        // 获取上次签到的日期
        string lastSignInDate = PlayerPrefs.GetString(LastSignInDateKey, "");

        // 获取当前日期
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // 如果当天没有签到，才允许增加天数
        if (lastSignInDate != currentDate)
        {

            // 假设从存档或玩家数据获取已存的所有数据
            int allDays = PlayerPrefs.GetInt(AllDaysKey, 0); // 默认从第1轮（0~10天）
            int startDays = PlayerPrefs.GetInt(StartDaysKey, 0);
            int finishDays = PlayerPrefs.GetInt(FinishDaysKey, 10);
            friendShipData = new FriendShipData(allDays, startDays, finishDays);
            SignIn();
            // 存储当天签到日期
            PlayerPrefs.SetString(LastSignInDateKey, currentDate);

        }
        else
        {
            // 如果当天已经签到，就不做任何操作
            friendShipData = new FriendShipData(PlayerPrefs.GetInt(AllDaysKey, 0), PlayerPrefs.GetInt(StartDaysKey, 0), PlayerPrefs.GetInt(FinishDaysKey, 10));
            UpdateUI();
        }
    }

    // 每次签到调用此方法
    public void SignIn()
    {
        // 增加总天数
        friendShipData.AllDate++;
        UpdateRound();
        UpdateUI();

        // 存储新的总天数
        PlayerPrefs.SetInt(AllDaysKey, friendShipData.AllDate);



    }


    // 更新进度条和开始结束日期
    private void UpdateRound()
    {
        // 计算当前轮次的开始和结束天数
        int startRound = (friendShipData.AllDate / 10) * 10;
        int finishRound = startRound + 10;

        friendShipData.StartDay = startRound;
        friendShipData.FinishDay = finishRound;
        // 存储新的天数
        PlayerPrefs.SetInt(StartDaysKey, friendShipData.StartDay);
        PlayerPrefs.SetInt(FinishDaysKey, friendShipData.FinishDay);

    }

    // 更新UI显示
    private void UpdateUI()
    {
        // 显示总天数
        alldate.text = friendShipData.AllDate.ToString();

        // 显示开始时间和结束时间
        startdate.text = friendShipData.StartDay.ToString();
        finishdate.text = friendShipData.FinishDay.ToString();

        // 更新进度条
        friendshipslider.value = (float)(friendShipData.AllDate % 10) / 10f;

        // 定义颜色
        ColorUtility.TryParseHtmlString("#D6D6D6", out Color defaultColor);
        ColorUtility.TryParseHtmlString("#F5904D", out Color highlightedColor);

        // 点亮图片
        for (int i = 0; i < imagelist.Count; i++)
        {
            if (friendshipslider.value >= (i + 1) * 0.2f)  // 每增加0.2，点亮一个图片
            {
                imagelist[i].color = highlightedColor;  // 激活图像
            }
            else
            {
                imagelist[i].color = defaultColor; // 禁用图像
            }
        }
    }
}