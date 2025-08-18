using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerMatchCalculator : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    private Vector2 referenceResolution;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        referenceResolution = canvasScaler.referenceResolution;

        CalculateAndSetMatchValue();
    }

    void CalculateAndSetMatchValue()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float referenceRatio = referenceResolution.x / referenceResolution.y;

        if (screenRatio > referenceRatio)
        {
            // 屏幕更宽，按高度适配，Match值接近1
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else if (screenRatio < referenceRatio)
        {
            // 屏幕更高，按宽度适配，Match值接近0
            canvasScaler.matchWidthOrHeight = 0f;
        }
        else
        {
            // 宽高比相同，使用默认的0.5
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
}