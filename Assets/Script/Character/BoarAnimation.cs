using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarAnimation : MonoBehaviour
{
    private Animator animator;
    public Boar boar;
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
        animator.SetBool("IsMoving", boar.isMoving);
        animator.SetBool("IsDead", !boar.IsAlive);
    }
    public void OnDead()
    {
        Destroy(boar.gameObject, 0.5f);
    }
}
