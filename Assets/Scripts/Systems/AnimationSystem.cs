using Component;
using Mono;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using AnimationState = Component.AnimationState;

namespace Systems
{
    public partial class AnimationSystem : SystemBase
    {
        private EntityManager _entityManager;
        
        protected override void OnCreate()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            SetAnimationStateOnGOs();
        }

        private void SetAnimationStateOnGOs()
        {
            foreach (var (animationState, e) in SystemAPI
                         .Query<RefRW<AnimationState>>()
                         .WithEntityAccess())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                
                var animator = _entityManager.GetComponentObject<AnimatorController>(e);
                animator.ChangeAnimation(animationState.ValueRO.Value);
            }
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct AnimationStateDiscoverySystem : ISystem
    {
        //get inputs from the ThirdPersonPlayerInputs component
        //Match those with the walk, jump, idle, and attack animations
        //modify s

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            DiscoverPlayerAnimationState(ref state);
        }

        [BurstCompile]
        private void DiscoverPlayerAnimationState(ref SystemState state)
        {
            foreach (var (animationState, characterBody) in SystemAPI
                         .Query<RefRW<AnimationState>, RefRO<KinematicCharacterBody>>())
            {
                var currentState = animationState.ValueRO.Value;
                var isGrounded = characterBody.ValueRO.IsGrounded;
                var velocity = math.length(characterBody.ValueRO.RelativeVelocity);
                var idleWalkThreshold = 0.5f;
                var walkRunThreshold = 1.5f;

                if (!isGrounded)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Jump;
                }
                else if (velocity < idleWalkThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Idle;
                }
                else if (velocity > idleWalkThreshold && velocity < walkRunThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Walk;
                }
                else if (velocity > walkRunThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Run;
                }
                
                animationState.ValueRW.HasChangedThisFrame = 
                    animationState.ValueRO.Value != currentState;
            }
        }
    }
}