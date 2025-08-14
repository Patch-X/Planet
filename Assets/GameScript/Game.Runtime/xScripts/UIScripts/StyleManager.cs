using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class StyleManager : MonoBehaviour
{
    public GameObject StylePrefab;          // 预制体
    public Image ModelImage;
    public Transform ListStyleParent;       // 列表父物体
     public Transform ListClothesParent;       // 列表父物体
    public List<StyleData> ColorData;//数据列表
    public List<StyleData> HairData;
    public List<StyleData> CheeksData;
    public List<StyleData> EyesData;
    public List<StyleData> VoicesData;
    public List<StyleData> GiftsData;
    public List<StyleData> TopData;
    public List<StyleData> BottomData;
    public List<StyleData> ScarfData;
    public List<StyleData> ShoesData;
    public List<StyleData> HeadData;
    public List<StyleData> FaceData;
    void Start()
    {
       
        RefreshStyle();
    }
    public void OnClickCreatStyle(String Style)
    {
        DestroyStyle();
        switch (Style)
        {
            case "Color":
            // 自动生成若干个任务
        for (int i = 0; i < ColorData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(ColorData,i);
        }
         break; 
        case "Hair":
            // 自动生成若干个任务
        for (int i = 0; i < HairData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(HairData,i);
        }
         break; 
        case "Cheeks":
            // 自动生成若干个任务
        for (int i = 0; i < CheeksData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(CheeksData,i);
        }
         break;
    case "Eyes":
            // 自动生成若干个任务
        for (int i = 0; i < EyesData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(EyesData,i);
        }
        break;
    case "Voices":
            // 自动生成若干个任务
        for (int i = 0; i < VoicesData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(VoicesData,i);
        }
        break; 
        case "Gifts":
        for (int i = 0; i < GiftsData.Count; i++) // 根据文本列表生成任务
        {
           CreatClothes(GiftsData,i);
      }
      break;
      case "Top":
        for (int i = 0; i < TopData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(TopData,i);
      }
      break;
      case "Bottom":
        for (int i = 0; i < BottomData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(BottomData,i);
      }
      break;
      case "Scarf":
        for (int i = 0; i < ScarfData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(ScarfData,i);
      }
      break;
      case "Shoes":
        for (int i = 0; i < ShoesData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(ShoesData,i);
      }
      break;
      case "Head":
        for (int i = 0; i < HeadData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(HeadData,i);
      }
      break;
      case "Face":
        for (int i = 0; i < FaceData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(FaceData,i);
      }
      break;
    }
    }
    private void CreatStyle(List<StyleData> styleData,int i)
    {
         GameObject newStyle = Instantiate(StylePrefab, ListStyleParent);//实例化预制体
         Button button = newStyle.GetComponent<Button>();
       
         Color color;
        
         if (ColorUtility.TryParseHtmlString(styleData[i].ImageColor, out color))
        {
            newStyle.GetComponent<Image>().color= color;  // 将解析后的颜色应用到图标
             button.onClick.AddListener(() => ModelImage.color=color);
        }
        else
        {
            newStyle.GetComponent<Image>().color= Color.white;  // 如果解析失败，使用默认颜色
        }
    }
    private void CreatClothes(List<StyleData> styleData,int i)
    {
         GameObject newClothes = Instantiate(StylePrefab, ListClothesParent);//实例化预制体
         Button button = newClothes.GetComponent<Button>();
       
         Color color;
        
         if (ColorUtility.TryParseHtmlString(styleData[i].ImageColor, out color))
        {
             newClothes.GetComponent<Image>().color= color;  // 将解析后的颜色应用到图标
             button.onClick.AddListener(() => ModelImage.color=color);
        }
        else
        {
             newClothes.GetComponent<Image>().color= Color.white;  // 如果解析失败，使用默认颜色
        }
    }
    public void RefreshStyle()
    {
         DestroyStyle();
          for (int i = 0; i < ColorData.Count; i++) // 根据文本列表生成任务
        {
            CreatStyle(ColorData,i);
        }
         for (int i = 0; i < GiftsData.Count; i++) // 根据文本列表生成任务
        {
            CreatClothes(GiftsData,i);
        }
    }
    public void DestroyStyle()
    {
            // 清空现有的预制体
    foreach (Transform child in ListStyleParent)
    {
        Destroy(child.gameObject);  // 删除所有子物体
    }
    foreach (Transform child in ListClothesParent)
    {
        Destroy(child.gameObject);  // 删除所有子物体
    }
    }

}
