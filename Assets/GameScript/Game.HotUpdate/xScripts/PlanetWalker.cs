using UnityEngine;
using System.Collections.Generic;
public class PlanetWalker : MonoBehaviour
{
    [Header("References")]
    public Transform visualModel;
    private Animator animator;
    public GameObject targetDis;
    public Transform planetCenter;

    [Header("Movement")]
    public float moveSpeed = 1.0f;
    public float raycastDistance = 30f;
    public LayerMask groundMask;

    [Header("Avoidance Settings")]
    public LayerMask obstacleMask;
    public float avoidanceRayLength = 10.0f;
    public float avoidanceAngle = 30f;

    [Header("Stuck Detection")]
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private float lastDistanceToTarget;
    private float stuckTimer = 0f;
    private float stuckThreshold = 0.02f;
    private float maxStuckTime = 2f;

    [Header("Idle Animation")]
    private float idleTimer = 0f;
    private float idleWaitThreshold = 1f;
    private int lastIdleIndex = -1;

    private bool isWaitingAtTarget = false;
    private float waitAtTargetTimer = 0f;
    private float waitAtTargetDuration = 2f;

    void Start()
    {
        animator = visualModel.GetComponent<Animator>();
        lastPosition = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        float reachDis = 0.1f;

        Vector3 downDir = (planetCenter.position - transform.position).normalized;
        Vector3 rayOrigin = transform.position - downDir * 0.5f;

        if (!Physics.SphereCast(rayOrigin, 0.2f, downDir, out RaycastHit hit, raycastDistance, groundMask))
            return;

        float distToTarget = Vector3.Distance(transform.position, targetPosition);
        bool shouldMove = distToTarget > reachDis && !isWaitingAtTarget;

        // === 计算绕障方向 ===
        Vector3 surfaceForward = Vector3.ProjectOnPlane((targetPosition - transform.position), transform.up).normalized;
        Vector3 moveDir = CalculateAvoidanceDirection(surfaceForward);

        // === 旋转朝向 ===
        if (shouldMove && moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            visualModel.rotation = Quaternion.Slerp(visualModel.rotation, targetRot, Time.deltaTime * 5f);

            float turnAngle = Vector3.SignedAngle(transform.forward, moveDir, transform.up);
            animator.SetFloat("Speed", turnAngle / 90f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // === 移动 ===
        if (shouldMove)
        {
            if (moveDir == Vector3.zero)
            {
                animator.SetFloat("Speed", 0f);
                return;
            }

            Vector3 moveAttempt = transform.position + moveDir * moveSpeed * Time.deltaTime;
            Vector3 downDirNew = (planetCenter.position - moveAttempt).normalized;
            Vector3 rayOriginNew = moveAttempt - downDirNew * 0.5f;

            if (Physics.Raycast(rayOriginNew, downDirNew, out RaycastHit moveHit, raycastDistance, groundMask))
            {
                transform.position = moveHit.point;
                transform.up = -downDirNew;
            }
            else
            {
                transform.position = moveAttempt;
                transform.up = -downDirNew;
            }
        }

        // === 到达目标点逻辑 ===
        if (distToTarget < reachDis)
        {
            if (!isWaitingAtTarget)
            {
                isWaitingAtTarget = true;
                waitAtTargetTimer = 0f;
                animator.SetBool("IsMoving", false);
            }
            else
            {
                waitAtTargetTimer += Time.deltaTime;
                if (waitAtTargetTimer > waitAtTargetDuration)
                {
                    if (Random.value < 0.5f)
                    {
                        PickNewTarget();
                        isWaitingAtTarget = false;
                    }
                    waitAtTargetTimer = 0f;
                }
            }
        }

        // === 动画管理 ===
        animator.SetBool("IsMoving", moveDir.sqrMagnitude > 0.01f);
        if (!shouldMove)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleWaitThreshold)
            {
                idleTimer = 0f;
                int newIndex = Random.Range(0, 2);
                animator.SetInteger("RandomIdle", newIndex);
                animator.SetBool("DoWait", true);
            }
        }
        else
        {
            idleTimer = 0f;
            animator.SetBool("DoWait", false);
        }

        // === 卡住检测 ===
        float currentDistance = Vector3.Distance(transform.position, targetPosition);
        bool notMovingMuch = (transform.position - lastPosition).sqrMagnitude < stuckThreshold * stuckThreshold;
        bool notGettingCloser = Mathf.Abs(currentDistance - lastDistanceToTarget) < 0.001f;

        if (shouldMove && notMovingMuch && notGettingCloser)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer > maxStuckTime * 3f)
            {
                if (!TryTeleportNearby())
                    PickNearbyTarget();
                stuckTimer = 0f;
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

        lastPosition = transform.position;
        lastDistanceToTarget = currentDistance;
    }

    Vector3 CalculateAvoidanceDirection(Vector3 directionToTarget)
    {
        Vector3 origin = transform.position;

        Debug.DrawRay(origin, directionToTarget * avoidanceRayLength, Color.red);

        if (Physics.Raycast(origin, directionToTarget, avoidanceRayLength, obstacleMask))
        {
            Vector3 planetUp = (transform.position - planetCenter.position).normalized;

            Vector3 rightDir = Quaternion.AngleAxis(avoidanceAngle, planetUp) * directionToTarget;
            Debug.DrawRay(origin, rightDir * avoidanceRayLength, Color.green);

            if (!Physics.Raycast(origin, rightDir, avoidanceRayLength, obstacleMask))
                return rightDir;

            Vector3 leftDir = Quaternion.AngleAxis(-avoidanceAngle, planetUp) * directionToTarget;
            Debug.DrawRay(origin, leftDir * avoidanceRayLength, Color.yellow);

            if (!Physics.Raycast(origin, leftDir, avoidanceRayLength, obstacleMask))
                return leftDir;

            return Vector3.zero;
        }

        return directionToTarget;
    }

    void showTargetBall()
    {
        if (targetDis != null)
            targetDis.transform.position = targetPosition;
    }

    void PickNewTarget()
    {
        Debug.Log("PickNewTarget");
        float maxAngle = 20f;
        Vector3 currentDir = (transform.position - planetCenter.position).normalized;
        Quaternion randomRot = Quaternion.AngleAxis(Random.Range(-maxAngle, maxAngle), Random.onUnitSphere);
        Vector3 newDir = randomRot * currentDir;

        Vector3 rayOrigin = planetCenter.position + newDir * 100f;
        if (Physics.Raycast(rayOrigin, -newDir, out RaycastHit hit, 200f, groundMask))
        {
            targetPosition = hit.point + hit.normal * 0.1f;
        }
        else
        {
            targetPosition = planetCenter.position + newDir * 10f;
        }


        targetPosition = TestTarget();

        showTargetBall();
    }

    Vector3 TestTarget()
    {
        if (GridManager.Instance.TryGetRandomAvailableCell(out GridCell cell))
        {
            Vector3 targetPosition = cell.worldPosition;
            return targetPosition;
        }
        else
        {
            return new Vector3(-50.0f, 0.0f, 0.0f);
        }
    }

    void PickNearbyTarget()
    {
        Vector3 offset = Random.onUnitSphere * 2f;
        Vector3 dir = (transform.position + offset - planetCenter.position).normalized;
        Vector3 origin = planetCenter.position + dir * 100f;
        if (Physics.Raycast(origin, -dir, out RaycastHit hit, 200f, groundMask))
        {
            targetPosition = hit.point + hit.normal * 0.1f;
        }
        else
        {
            targetPosition = transform.position + offset;
        }

        showTargetBall();
    }

    bool TryTeleportNearby()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 offset = Random.onUnitSphere * 2f;
            Vector3 dir = (transform.position + offset - planetCenter.position).normalized;
            Vector3 origin = planetCenter.position + dir * 100f;
            if (Physics.Raycast(origin, -dir, out RaycastHit hit, 200f, groundMask))
            {
                transform.position = hit.point + hit.normal * 0.1f;
                targetPosition = hit.point;
                return true;
            }
        }
        return false;
    }

    void DoFindPath()
    {
        GridCell startCell = new GridCell();
        GridCell endCell = new GridCell();

        List<GridCell> path = Pathfinder.FindPath(startCell, endCell);
        if (path != null)
        {
            foreach (var cell in path)
            {
                Debug.Log($"Path through: face={cell.face}, x={cell.x}, y={cell.y}");
            }
        }
    }
}
