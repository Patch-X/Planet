using UnityEngine;

public class PlanetMaskController : MonoBehaviour
{
    public Material planetMaterial;
    [Range(0, 1)] public float threshold = 0f;

    void Update()
    {
        planetMaterial.SetFloat("_Threshold", threshold);

        // 测试：按空格逐渐扩大
        if (Input.GetKey(KeyCode.Space))
        {
            threshold += Time.deltaTime * 0.1f;
            threshold = Mathf.Clamp01(threshold);
        }
    }
}
