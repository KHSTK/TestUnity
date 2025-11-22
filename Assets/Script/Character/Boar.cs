using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 野猪类，继承自MonoBehaviour，用于实现野猪怪物的AI行为
/// </summary>
public class Boar : MonoBehaviour
{
    [Header("设置")]
    public float detectRange = 8f;    // 检测范围，怪物能发现玩家的距离
    public float attackRange = 2f;    // 攻击范围，怪物能攻击到玩家的距离
    public float attackAngle = 60f;   // 攻击角度，怪物能攻击到玩家的扇形角度
    public float moveSpeed = 2f;       // 移动速度，怪物移动的速度

    [Header("组件")]
    public Animator animator;         // 动画组件，用于控制怪物动画
    public Rigidbody rb;              // 刚体组件，用于物理移动
    private Character controller;     // 角色控制器组件，用于控制角色基本行为
    private Transform player;         // 玩家 Transform，用于获取玩家位置
    private MonsterState currentState; // 怪物当前状态
    public Vector3 direction;         // 移动方向
    public bool isMoving;             // 是否正在移动
    private bool isAlive = true;      // 是否存活
    private bool isAttacking = false; // 是否正在攻击
    /// <summary>
    /// 怪物状态枚举
    /// </summary>
    private enum MonsterState
    {
        Idle,    // 空闲状态
        Chase,   // 追击状态
        Attack,  // 攻击状态
        Dead     // 死亡状态
    }
    // 属性：获取是否存活的状态
    public bool IsAlive => isAlive;


    /// <summary>
    /// 唤醒时调用，初始化组件和状态
    /// </summary>
    private void Awake()
    {
        // 获取玩家Transform
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();
        // 获取角色控制器组件
        controller = GetComponent<Character>();
        // 初始状态设为空闲
        ChangeState(MonsterState.Idle);
    }
    /// <summary>
    /// 对象启用时调用，重置存活状态和初始状态
    /// </summary>
    void OnEnable()
    {
        isAlive = true;
        ChangeState(MonsterState.Idle);
    }
    /// <summary>
    /// 每帧调用，更新怪物状态
    /// </summary>
    private void Update()
    {
        // 更新存活状态
        isAlive = controller.isAlive;
        // 如果死亡，切换到死亡状态并返回
        if (!isAlive)
        {
            AudioManager.Instance.PlaySFX("Pig");
            Dead();
            return;
        }

        // 根据当前状态执行对应的更新逻辑
        switch (currentState)
        {
            case MonsterState.Idle:
                UpdateIdleState();
                break;
            case MonsterState.Chase:
                UpdateChaseState();
                break;
            case MonsterState.Attack:
                UpdateAttackState();
                break;
        }
    }

    /// <summary>
    /// 更新空闲状态
    /// </summary>
    private void UpdateIdleState()
    {
        isMoving = false;
        // 如果玩家在检测范围内，切换到追击状态
        if (PlayerInRange(detectRange))
        {
            ChangeState(MonsterState.Chase);
        }
    }

    /// <summary>
    /// 更新追击状态
    /// </summary>
    private void UpdateChaseState()
    {
        // 如果玩家不在检测范围内，切换回空闲状态
        if (!PlayerInRange(detectRange))
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 如果玩家在攻击扇区内，切换到攻击状态
        if (PlayerInAttackSector())
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        // 如果正在攻击，则不执行追击逻辑
        if (isAttacking) return;
        // 设置移动状态
        isMoving = true;
        // 计算朝向玩家的方向
        direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + direction.normalized * moveSpeed * Time.deltaTime);
            FaceTarget(direction);
        }
    }

    /// <summary>
    /// 更新怪物攻击状态的方法
    /// 检查玩家是否在攻击范围内，并决定是否开始攻击或切换状态
    /// </summary>
    private void UpdateAttackState()
    {
        // 检查玩家是否不在攻击扇区内
        if (!PlayerInAttackSector())
        {
            // 如果玩家不在攻击扇区内，切换到追击状态
            ChangeState(MonsterState.Chase);
            return;  // 提前退出该方法
        }

        // 检查怪物当前是否没有处于攻击状态
        if (!isAttacking)
        {
            // 如果没有在攻击，则开始攻击
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(ResetAfterAttack());
    }

    /// <summary>
    /// 攻击结束后重置状态的协程方法
    /// </summary>
    /// <returns>返回一个用于协程的IEnumerator</returns>
    private IEnumerator ResetAfterAttack()
    {
        // 获取当前动画状态的持续时间长度
        float attackLength = animator.GetCurrentAnimatorStateInfo(0).length;
        // 等待攻击动画播放完毕
        yield return new WaitForSeconds(attackLength);

        // 将攻击状态设置为false
        isAttacking = false;

        // 检查玩家是否不在攻击范围内
        if (!PlayerInAttackSector())
        {
            // 如果玩家不在攻击范围内，切换到追击状态
            ChangeState(MonsterState.Chase);
        }
    }

    private void ChangeState(MonsterState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (newState == MonsterState.Dead)
        {
            isAlive = false;
            Debug.Log("死亡");
            isAttacking = false;
            StopAllCoroutines();
        }
    }

    private bool PlayerInRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= range;
    }

    /// <summary>
    /// 检查玩家是否在攻击范围内
    /// </summary>
    /// <returns>如果玩家在攻击范围内则返回true，否则返回false</returns>
    private bool PlayerInAttackSector()
    {
        // 检查玩家对象是否存在，如果不存在则直接返回false
        if (player == null) return false;

        // 计算从当前对象到玩家的方向向量，并进行标准化
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // 计算当前对象与玩家之间的距离
        float distance = Vector3.Distance(transform.position, player.position);
        // 计算当前对象的前方方向与玩家方向之间的夹角
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 判断玩家是否在攻击距离和攻击角度范围内
        return distance <= attackRange && angle <= attackAngle * 0.5f;
    }

    /// <summary>
    /// 朝向
    /// </summary>
    /// <param name="direction">目标方向向量</param>
    private void FaceTarget(Vector3 direction)
    {
        // 将方向向量的Y分量设为0，确保物体只在水平面上旋转
        direction.y = 0;
        // 如果方向向量为零向量，则直接返回，不进行任何旋转
        if (direction == Vector3.zero) return;

        // 计算目标旋转四元数，使物体朝向指定方向
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 使用球面线性插值(Slerp)平滑地旋转物体到目标旋转
        // 5f * Time.deltaTime 控制旋转速度，5f是旋转速度系数，Time.deltaTime确保旋转速度与帧率无关
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    public void Dead()
    {
        ChangeState(MonsterState.Dead);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        DrawAttackSectorGizmo();
    }

    private void DrawAttackSectorGizmo()
    {
        int segments = 20;
        float halfAngle = attackAngle * 0.5f;

        Vector3[] points = new Vector3[segments + 2];
        points[0] = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (attackAngle / segments) * i;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            points[i + 1] = transform.position + direction * attackRange;
        }

        for (int i = 1; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
        Gizmos.DrawLine(transform.position, points[1]);
        Gizmos.DrawLine(transform.position, points[points.Length - 1]);
    }
}