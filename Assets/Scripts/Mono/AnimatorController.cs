using System;
using Component;
using UnityEngine;

namespace Mono
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform trans;

        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            trans.position = animator.transform.position;
        }

        public void ChangeAnimation(AnimationStateEnum state)
        {
            switch (state)
            {
                case AnimationStateEnum.Idle:
                    animator.SetBool(Idle, true);
                    animator.SetBool(Run, false);
                    animator.SetBool(Walk, false);
                    break;
                case AnimationStateEnum.Walk:
                    animator.SetBool(Walk, true);
                    animator.SetBool(Idle, false);
                    animator.SetBool(Run, false);
                    break;
                case AnimationStateEnum.Run:
                    animator.SetBool(Run, true);
                    animator.SetBool(Walk, false);
                    animator.SetBool(Idle, false);
                    break;
                case AnimationStateEnum.Jump:
                    animator.SetTrigger(Jump);
                    break;
                case AnimationStateEnum.Prone:
                    //
                    break;
                case AnimationStateEnum.Attack:
                    animator.SetTrigger(Attack);
                    break;
                case AnimationStateEnum.Fear:
                    //
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}