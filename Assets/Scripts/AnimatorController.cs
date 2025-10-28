using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform trans;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int WalkForward = Animator.StringToHash("Walk Forward");

    public Transform Transform => trans;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        trans.position = animator.transform.position;
    }

    public void MoveAnimation(float velocity)
    {
        animator.SetFloat(Speed, velocity, 0.1f, Time.deltaTime);
        animator.Play(WalkForward);
    }

    public void AttackAnimation()
    {
        animator.Play("Attack1");
    }
}