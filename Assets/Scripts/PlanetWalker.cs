using UnityEngine;

public class PlanetWalker : MonoBehaviour
{
    public Transform planetCenter;
    public float moveSpeed = 1.0f;
    public float raycastDistance = 30f;
    public LayerMask groundMask;

    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 0.01f;
    private float maxStuckTime = 2f;

    public Transform visualModel;
    private Animator animator;

    void Start()
    {
        PickNewTarget();
        lastPosition = transform.position;

        animator = visualModel.GetComponent<Animator>();
    }

    void Update()
    {

        Vector3 downDir = (planetCenter.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position - downDir * 0.5f;

        if (Physics.SphereCast(rayOrigin, 0.2f, downDir, out RaycastHit hit, raycastDistance, groundMask))
        {
            // 1. 贴地
            transform.position = Vector3.MoveTowards(transform.position, hit.point, Time.deltaTime * moveSpeed);
            transform.up = -downDir;

            // 2. 对齐前方方向
            Vector3 surfaceForward = Vector3.ProjectOnPlane((targetPosition - transform.position), transform.up).normalized;
            if (surfaceForward.sqrMagnitude > 0.01f)
            {
                // 若模型正面是 -Z，就加个负号
                Quaternion targetRot = Quaternion.LookRotation(surfaceForward, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }

            if (surfaceForward.sqrMagnitude > 0.01f && visualModel != null)
            {
                Quaternion targetRot = Quaternion.LookRotation(surfaceForward, transform.up);
                visualModel.rotation = Quaternion.Slerp(visualModel.rotation, targetRot, Time.deltaTime * 5f);
            }

            // 3. 前进
            transform.position += surfaceForward * moveSpeed * Time.deltaTime;

            // 4. 到达目标
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                PickNewTarget();
            }

            // 5. 卡住检测
            if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > maxStuckTime * 3)
                {
                    bool escaped = TryTeleportNearby();
                    if (escaped)
                    {
                        stuckTimer = 0f;
                    }
                    else
                    {
                        PickNearbyTarget(); // 传送失败，换目标
                    }
                }
                else if (stuckTimer > maxStuckTime)
                {
                    PickNearbyTarget();
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            // Debug.DrawRay(rayOrigin, downDir * hit.distance, Color.green);
            // Debug.DrawLine(transform.position, targetPosition, Color.yellow);
        }
        else
        {
            // Debug.DrawRay(rayOrigin, downDir * raycastDistance, Color.red);
            // Debug.Log("Raycast miss!");
        }

        // 计算速度（用于动画）
        float moveAmount = (transform.position - lastPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", moveAmount);
        animator.SetBool("IsMoving", moveAmount > 0.1f);
        // if (moveAmount > 0.001f)
        // {
        //     Debug.Log("Speed:" + moveAmount);
        // }

        lastPosition = transform.position;
    }

    void PickNewTarget()
    {
        float maxAngle = 20f;
        Vector3 currentDir = (transform.position - planetCenter.position).normalized;
        Quaternion randomRot = Quaternion.AngleAxis(Random.Range(-maxAngle, maxAngle), Random.onUnitSphere);
        Vector3 newDir = randomRot * currentDir;

        Ray ray = new Ray(planetCenter.position + newDir * 100f, -newDir);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = planetCenter.position + newDir * 10f;
        }
    }
    void PickNearbyTarget()
    {
        float perturbDistance = 2f; // 附近 2 米内
        Vector3 randomOffset = Random.onUnitSphere * perturbDistance;
        Vector3 probeDir = (transform.position + randomOffset - planetCenter.position).normalized;

        Ray ray = new Ray(planetCenter.position + probeDir * 100f, -probeDir);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            targetPosition = hit.point;
        }
        else
        {
            // fallback
            targetPosition = transform.position + randomOffset;
        }

        // Debug.Log("Re-picking nearby target");
    }
    bool TryTeleportNearby()
    {
        float radius = 2f; // 传送范围
        for (int i = 0; i < 10; i++)
        {
            Vector3 offset = Random.onUnitSphere * radius;
            Vector3 dir = (transform.position + offset - planetCenter.position).normalized;

            Ray ray = new Ray(planetCenter.position + dir * 100f, -dir);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
            {
                transform.position = hit.point + hit.normal * 0.1f; // 稍微抬高避免再次卡住
                targetPosition = hit.point;
                Debug.Log("Teleported to escape stuck.");
                return true;
            }
        }
        return false;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(targetPosition, 0.2f);
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(transform.position, targetPosition);
    // }
}
