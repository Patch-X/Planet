using UnityEngine;

public class PlanetWalker : MonoBehaviour
{
    public Transform planetCenter;
    public float moveSpeed = 1.0f;
    public float raycastDistance = 10f;
    public int a;
    public LayerMask groundMask;

    private Vector3 targetPosition;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        // 计算贴地方向
        Vector3 downDir = (planetCenter.position - transform.position).normalized;

        // 使用射线找到地面
        if (Physics.Raycast(transform.position, downDir, out RaycastHit hit, raycastDistance, groundMask))
        {
            transform.position = hit.point;
            transform.up = -downDir;

            // 计算“水平行走方向”，并设为 forward（避免角色朝错方向）
            Vector3 surfaceForward = Vector3.ProjectOnPlane((targetPosition - transform.position), transform.up).normalized;

            // 💡 添加这行代码，保证角色正面朝前走
            if (surfaceForward.sqrMagnitude > 0.01f)
                transform.forward = Vector3.Slerp(transform.forward, surfaceForward, Time.deltaTime * 5f);

            // 行走
            transform.position += surfaceForward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                PickNewTarget();
            }
        }

    }

    void PickNewTarget()
    {
        Vector3 randomDir = Random.onUnitSphere;
        float radius = Vector3.Distance(transform.position, planetCenter.position);
        targetPosition = planetCenter.position + randomDir * radius;
    }
}