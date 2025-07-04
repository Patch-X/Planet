using UnityEngine;

public class CameraModeSwitcher : MonoBehaviour
{
    public OrbitCamera orbitCamera;
    public FollowCamera followCamera;

    void Start()
    {
        SetFollowMode(false); // 默认是自由模式
    }

    void Update()
    {
        // 按 F 切换模式
        if (Input.GetKeyDown(KeyCode.F))
        {
            bool followEnabled = !followCamera.enabled;
            SetFollowMode(followEnabled);
        }
    }

    void SetFollowMode(bool enabled)
    {
        orbitCamera.enabled = !enabled;
        followCamera.enabled = enabled;
    }
}
