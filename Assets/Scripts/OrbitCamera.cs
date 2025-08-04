using UnityEngine;

[ExecuteInEditMode]
public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 50.0f;
    public float rotationSpeed = 120.0f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 100.0f;

    private Quaternion currentRotation;

    void Start()
    {
        if (target == null) return;
        currentRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion deltaRotation = Quaternion.identity;

        // === 鼠标控制 ===
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
        if (Input.GetMouseButton(0))
        {
            float deltaX = Input.GetAxis("Mouse X") * rotationSpeed * 0.02f;
            float deltaY = Input.GetAxis("Mouse Y") * rotationSpeed * 0.02f;

            deltaRotation *= Quaternion.AngleAxis(-deltaY, transform.right);  // 上下旋转
            deltaRotation *= Quaternion.AngleAxis(deltaX, transform.up);      // ✅ 改为相机自身 Up，实现视觉上的左右旋转
        }

        if (Input.GetMouseButton(1))
        {
            float deltaZ = Input.GetAxis("Mouse X") * rotationSpeed * 0.02f;
            deltaZ += Input.GetAxis("Mouse Y") * rotationSpeed * 0.02f;

            deltaRotation *= Quaternion.AngleAxis(deltaZ, transform.forward); // 倾斜
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
#endif

        // === 触摸控制 ===
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;

                deltaRotation *= Quaternion.AngleAxis(-delta.y * rotationSpeed * 0.002f, transform.right);
                deltaRotation *= Quaternion.AngleAxis(delta.x * rotationSpeed * 0.002f, transform.up); // ✅ 视觉左右旋转
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 currDist = touch1.position - touch2.position;
            Vector2 prevDist = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);
            float deltaMagnitude = currDist.magnitude - prevDist.magnitude;
            distance -= deltaMagnitude * zoomSpeed * 0.01f;

            Vector2 prevDir = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);
            Vector2 currDir = touch1.position - touch2.position;
            float angleDelta = Vector2.SignedAngle(prevDir, currDir);
            deltaRotation *= Quaternion.AngleAxis(angleDelta, transform.forward); // 倾斜
        }

        // === 应用旋转与缩放限制 ===
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        currentRotation = deltaRotation * currentRotation;

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = currentRotation * negDistance + target.position;

        transform.rotation = currentRotation;
        transform.position = position;
    }
}
