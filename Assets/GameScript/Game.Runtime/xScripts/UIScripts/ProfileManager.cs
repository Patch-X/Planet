using UnityEngine;
using UnityEngine.UI;
public class ProfileManager : MonoBehaviour
{
    public RectTransform Scrolls;
    public Text usernametext;
    public InputField usernameinput;
    public Text tolannametext;
    public InputField tolannameinput;
    public Text phonenumbertext;
    public InputField phonenumberinput;
    public Text ismusictext;
    public Text allnametext;
    private ProfileData profiledata;
    private string username;
    private string tolanname;
    private string phonenumber;
    private bool ismusic;
    private PanelDOTween panelDOTween;
    void Start()
    {
        panelDOTween = GetComponent<PanelDOTween>();
        username = PlayerPrefs.GetString("username", "");
        tolanname = PlayerPrefs.GetString("tolanname", "");
        phonenumber = PlayerPrefs.GetString("phonenumber", "");
        if (PlayerPrefs.GetInt("ismusic", 1) == 1)
        {
            ismusic = true;
            ismusictext.text = "On";
        }
        else
        {
            ismusic = false;
            ismusictext.text = "Off";
        }
        // 将 1 或 0 转换为布尔值
        usernametext.text = username;
        tolannametext.text = tolanname;
        phonenumbertext.text = phonenumber;
        profiledata = new ProfileData(username, tolanname, phonenumber, ismusic);
        panelDOTween.OnSwitchClick(Scrolls, profiledata.ismusic);
        Setallnametext();
    }
    public void Setusernametext()
    {
        if (!string.IsNullOrEmpty(usernameinput.text))
        {
            username = usernameinput.text;
            PlayerPrefs.SetString("username", username);
            usernametext.text = username;
            profiledata = new ProfileData(username, tolanname, phonenumber, ismusic);
            Setallnametext();
        }

    }
    public void Settolannametext()
    {
        if (!string.IsNullOrEmpty(tolannameinput.text))
        {
            tolanname = tolannameinput.text;
            PlayerPrefs.SetString("tolanname", tolanname);
            tolannametext.text = tolanname;
            profiledata = new ProfileData(username, tolanname, phonenumber, ismusic);
            Setallnametext();
        }

    }
    public void Setphonenumbertext()
    {
        if (!string.IsNullOrEmpty(phonenumberinput.text))
        {
            phonenumber = phonenumberinput.text;
            PlayerPrefs.SetString("phonenumber", phonenumber);
            phonenumbertext.text = phonenumber;
            profiledata = new ProfileData(username, tolanname, phonenumber, ismusic);
        }
    }
    public void Setismusictext()
    {
        if (profiledata.ismusic)
        {
            ismusic = false;
            ismusictext.text = "Off";
            PlayerPrefs.SetInt("ismusic", 0);

        }
        else
        {
            ismusic = true;
            ismusictext.text = "On";
            PlayerPrefs.SetInt("ismusic", 1);
        }
        profiledata = new ProfileData(username, tolanname, phonenumber, ismusic);
        panelDOTween.OnSwitchClick(Scrolls, profiledata.ismusic);
    }
    private void Setallnametext()
    {
        allnametext.text = profiledata.username + "&" + profiledata.tolanname;
    }


}
