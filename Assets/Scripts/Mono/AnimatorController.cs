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
                    UnsetAll(animator);
                    animator.SetBool(Idle, true);
                    break;
                case AnimationStateEnum.Walk:
                    UnsetAll(animator);
                    animator.SetBool(Walk, true);
                    break;
                case AnimationStateEnum.Run:
                    UnsetAll(animator);
                    animator.SetBool(Run, true);
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
                    UnsetAll(animator);
                    animator.SetBool(Idle, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void UnsetAll(Animator anim)
        {
            anim.SetBool(Walk, false);
            anim.SetBool(Idle, false);
            anim.SetBool(Run, false);
            anim.SetBool(Jump, false);
        }
    }
}