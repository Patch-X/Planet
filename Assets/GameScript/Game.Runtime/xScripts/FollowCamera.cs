using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;                      // 目标角色
    public Vector3 offset = new Vector3(0, 5, -10); // 摄像机与角色之间的偏移
    public float followSmoothTime = 0.1f;         // 平滑跟随时间
    public float rotationSmoothTime = 5f;         // 平滑旋转速度

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 平滑位置跟随
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSmoothTime);

        // 平滑旋转朝向
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothTime * Time.deltaTime);
        }
    }
}
