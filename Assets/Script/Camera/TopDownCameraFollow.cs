using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("目标设置")]
    public Transform target;
    public string targetTag = "Player"; // 通过标签查找

    [Header("摄像头位置")]
    public float height = 15f;         // 摄像头高度
    public float distance = 10f;     // 与目标的水平距离
    public float angle = 45f;         // 俯视角度

    [Header("跟随参数")]
    public float smoothSpeed = 5f;
    public bool rotateWithTarget = false;

    [Header("边界限制")]
    public bool enableBounds = false;
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);

    private Vector3 currentVelocity;


    private void Start()
    {
        // 自动查找主角
        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
            if (playerObj != null)
                target = playerObj.transform;
        }

        // 初始位置设置
        if (target != null)
        {
            InitializeCameraPosition();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        FollowTarget();
    }

    private void InitializeCameraPosition()
    {
        // 计算初始位置
        Vector3 targetPosition = target.position;
        Vector3 cameraPosition = CalculateCameraPosition(targetPosition);

        transform.position = cameraPosition;
        transform.rotation = CalculateCameraRotation();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = target.position;

        // 应用边界限制
        if (enableBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        }

        // 计算目标摄像头位置
        Vector3 desiredPosition = CalculateCameraPosition(targetPosition);

        // 平滑移动
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition,
                                              ref currentVelocity, smoothSpeed * Time.deltaTime);

        // 设置旋转
        transform.rotation = CalculateCameraRotation();
    }

    private Vector3 CalculateCameraPosition(Vector3 targetPos)
    {
        // 计算偏移方向
        Vector3 offsetDirection = new Vector3(0, Mathf.Sin(angle * Mathf.Deg2Rad),
                                            -Mathf.Cos(angle * Mathf.Deg2Rad));
        offsetDirection = offsetDirection.normalized * distance;

        // 计算最终位置
        Vector3 cameraPos = targetPos + new Vector3(0, height, -3) + offsetDirection;
        return cameraPos;
    }

    private Quaternion CalculateCameraRotation()
    {
        // 计算俯视角度的旋转
        return Quaternion.Euler(angle, 0, 0);
    }

    // 通过事件设置目标
    public void SetCameraTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            InitializeCameraPosition();
        }
    }

    // 在编辑器中可视化摄像头范围
    private void OnDrawGizmosSelected()
    {
        if (target != null && enableBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) * 0.5f,
                target.position.y,
                (minBounds.y + maxBounds.y) * 0.5f
            );
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
            Gizmos.DrawWireCube(center, size);
        }
    }
}