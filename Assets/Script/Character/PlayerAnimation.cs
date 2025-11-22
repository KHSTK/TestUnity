using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    public PlayerController playerController;
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    private void Update()
    {
        SetAnimation();
    }
    public void SetAnimation()
    {
        animator.SetFloat("MoveSpeed", playerController.moveDirection.magnitude);
        animator.SetBool("IsAttack", playerController.isAttacking);
        animator.SetBool("IsDead", playerController.isDead);
    }
    public void OnDead()
    {
        playerController.gameObject.transform.position = new Vector3(0, 0, 0);
        playerController.gameObject.SetActive(false);
    }

}
