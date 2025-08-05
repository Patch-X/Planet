using UnityEngine;

public class ProfileData
{
    public string username;
    public string tolanname;
    public string phonenumber;
    public bool ismusic;
    public ProfileData(string username, string tolanname, string phonenumber, bool ismusic)
    {
        this.username = username;
        this.tolanname = tolanname;
        this.phonenumber = phonenumber;
        this.ismusic = ismusic;
    }
}
