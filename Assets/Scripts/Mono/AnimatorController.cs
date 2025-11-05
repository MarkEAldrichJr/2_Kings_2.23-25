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

        public Transform Transform => trans;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            trans.position = animator.transform.position;
        }

        public void ChangeAnimation(AnimationStateEnum state)
        {
            Debug.Log($"Changing animation: {state}");
            switch (state)
            {
                case AnimationStateEnum.Idle:
                    animator.Play("Idle");
                    break;
                case AnimationStateEnum.Walk:
                    animator.Play("Walk Forward");
                    break;
                case AnimationStateEnum.Run:
                    animator.Play("RunForward");
                    break;
                case AnimationStateEnum.Jump:
                    break;
                case AnimationStateEnum.Sleep:
                    break;
                case AnimationStateEnum.Crouch:
                    break;
                case AnimationStateEnum.Prone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}