// FriendShipData.cs
public class FriendShipData
{
    public int AllDate;  // 总天数
    public int StartDay; // 当前轮次的开始天数
    public int FinishDay; // 当前轮次的结束天数

    // Constructor
    public FriendShipData(int allDate, int startDay, int finishDay)
    {
        AllDate = allDate;
        StartDay = startDay;
        FinishDay = finishDay;
    }
}