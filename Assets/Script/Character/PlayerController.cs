using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float attackRange = 2f;

    [Header("组件引用")]
    public Rigidbody rb;
    public RockerController rocker;
    public Vector3 moveDirection;
    public bool isAttacking;
    public bool isDead;
    private Character character;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rocker == null)
            rocker = FindObjectOfType<RockerController>();
        character = GetComponent<Character>();
    }
    private void OnEnable()
    {
        character?.Init();
        isDead = false;
        isAttacking = false;
    }
    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void HandleInput()
    {
        // 从摇杆获取输入
        moveDirection = new Vector3(rocker.outPos.x, 0, rocker.outPos.y);
    }
    private void MoveCharacter()
    {
        if (isDead) return;
        if (moveDirection.magnitude > 0.1f)
        {
            // 移动角色
            Vector3 movement = moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            // 面向移动方向
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime));
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        isAttacking = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isAttacking = false;
    }
    public void Dead()
    {
        isDead = true;
    }

}