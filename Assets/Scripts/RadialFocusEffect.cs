using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class RadialFocusEffect : MonoBehaviour
{
    public Material effectMaterial;
    public Transform focusTarget; // ⭐ 星球模型中心
    public float softness = 0.1f;
    public Color darkColor = Color.black;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial == null || focusTarget == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        // 计算 focusTarget 在屏幕上的位置（0-1范围）
        Vector3 viewportPos = cam.WorldToViewportPoint(focusTarget.position);
        effectMaterial.SetVector("_Center", new Vector4(viewportPos.x, viewportPos.y, 0, 0));

        // 计算星球模型的“屏幕半径”作为动态半径
        float worldRadius = 7.5f; // ❗根据你模型实际半径调整（或用 bounds 动态获取）
        Vector3 edgeWorld = focusTarget.position + cam.transform.right * worldRadius;
        Vector3 edgeViewport = cam.WorldToViewportPoint(edgeWorld);
        float dynamicRadius = Vector2.Distance(new Vector2(viewportPos.x, viewportPos.y), new Vector2(edgeViewport.x, edgeViewport.y));
        effectMaterial.SetFloat("_Radius", dynamicRadius);

        effectMaterial.SetFloat("_Softness", softness);
        effectMaterial.SetColor("_DarkColor", darkColor);

        Graphics.Blit(src, dest, effectMaterial);
    }
}
