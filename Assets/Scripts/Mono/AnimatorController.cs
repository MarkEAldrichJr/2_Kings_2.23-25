using System;
using Component;
using UnityEngine;

namespace Mono
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform trans;

        private static readonly int WalkForward = Animator.StringToHash("Walk Forward");
        private static readonly int Idle = Animator.StringToHash("Combat Idle");
        private static readonly int RunForward = Animator.StringToHash("Run Forward");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Attack1 = Animator.StringToHash("Attack1");

        public Transform Transform => trans;

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
                    animator.SetBool(RunForward, false);
                    animator.SetBool(WalkForward, false);
                    break;
                case AnimationStateEnum.Walk:
                    animator.SetBool(WalkForward, true);
                    animator.SetBool(Idle, false);
                    animator.SetBool(RunForward, false);
                    break;
                case AnimationStateEnum.Run:
                    animator.SetBool(RunForward, true);
                    animator.SetBool(WalkForward, false);
                    animator.SetBool(Idle, false);
                    break;
                case AnimationStateEnum.Jump:
                    animator.SetTrigger(Jump);
                    break;
                case AnimationStateEnum.Prone:
                    //
                    break;
                case AnimationStateEnum.Attack:
                    animator.SetTrigger(Attack1);
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