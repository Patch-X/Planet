using UnityEngine;
using System;
[System.Serializable]
public class ReminderData
{
    public string text;
    public string Hour;
    public string Minute;
    public int ID;
    public int time;//时间总和，用于计算排序

    public ReminderData(string text, string Hour, string Minute, int ID)
    {
        this.text = text;
        this.Hour = Hour;
        this.Minute = Minute;
        this.ID = ID;
        // ID = GetNextId();
        time = int.Parse(Hour) * 60 + int.Parse(Minute);


    }
    // private static int nextId = 1;
    // private static int GetNextId()
    // {
    //     return nextId++;
    // }

}
