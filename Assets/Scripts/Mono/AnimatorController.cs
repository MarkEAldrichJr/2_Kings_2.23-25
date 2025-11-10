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
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int RunForward = Animator.StringToHash("RunForward");

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
                    animator.Play(Idle);
                    break;
                case AnimationStateEnum.Walk:
                    animator.Play(WalkForward);
                    break;
                case AnimationStateEnum.Run:
                    animator.Play(RunForward);
                    break;
                case AnimationStateEnum.Jump:
                    //
                    break;
                case AnimationStateEnum.Sleep:
                    //
                    break;
                case AnimationStateEnum.Crouch:
                    //
                    break;
                case AnimationStateEnum.Prone:
                    //
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}