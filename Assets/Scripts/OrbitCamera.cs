using UnityEngine;

[ExecuteInEditMode]
public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 30.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 40.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        if (target == null) return;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // === 鼠标控制（编辑器） ===
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            if(Input.GetAxis("Mouse X") != 0)
            {
                Debug.Log(Input.GetAxis("Mouse X"));
            }
            
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
#endif

        // === 触摸控制（移动端） ===
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                x += delta.x * xSpeed * 0.002f;
                y -= delta.y * ySpeed * 0.002f;
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
        }

        // === 限制角度和缩放范围 ===
        // y = Mathf.Clamp(y, -85f, 85f);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // === 相机位置计算 ===
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
