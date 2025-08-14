using UnityEngine;
using System;
[System.Serializable]
public class ReminderData
{
    public string text;
    public string firetimeString; // 用于序列化
    public int ID;
    [NonSerialized]
    public DateTime firetime; // 不参与序列化
    public ReminderData(string text, DateTime firetime, int ID)
    {
        this.text = text;
        this.firetime = firetime;
        firetimeString = firetime.ToString("yyyy-MM-dd HH:mm:ss");
        this.ID = ID;
    }
    public void ParseFiretime()
    {
        firetime = DateTime.Parse(firetimeString);
    }

}
