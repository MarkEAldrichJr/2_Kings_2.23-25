using Component;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Mono;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
                         .Query<RefRW<AnimationStateComp>>()
                         .WithEntityAccess())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                
                var animator = _entityManager.GetComponentObject<AnimatorController>(e);
                animator.ChangeAnimation(animationState.ValueRO.Value);

                animationState.ValueRW.HasChangedThisFrame = false;
            }
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct AnimationStateDiscoverySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var playerBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimationStateComp, KinematicCharacterBody>();
            state.RequireForUpdate(state.GetEntityQuery(playerBuilder));

            var npcBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimationStateComp>();
            state.RequireForUpdate(state.GetEntityQuery(npcBuilder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            DiscoverPlayerAnimationState(ref state);
            DiscoverNpcAnimationState(ref state);
        }

        [BurstCompile]
        private void DiscoverPlayerAnimationState(ref SystemState state)
        {
            foreach (var (animationState, characterBody, characterControl) in SystemAPI
                         .Query<RefRW<AnimationStateComp>, RefRO<KinematicCharacterBody>, RefRW<ThirdPersonCharacterControl>>())
            {
                var currentState = animationState.ValueRO.Value;
                var isGrounded = characterBody.ValueRO.IsGrounded;
                var velocity = math.length(characterBody.ValueRO.RelativeVelocity);
                
                if (characterControl.ValueRO.Attack)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Attack;
                }
                else if (!isGrounded)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Jump;
                }
                else if (velocity < animationState.ValueRO.IdleWalkThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Idle;
                }
                else if (velocity > animationState.ValueRO.IdleWalkThreshold &&
                         velocity < animationState.ValueRO.WalkRunThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Walk;
                }
                else if (velocity > animationState.ValueRO.WalkRunThreshold)
                {
                    animationState.ValueRW.Value = AnimationStateEnum.Run;
                }
                
                animationState.ValueRW.HasChangedThisFrame = 
                    animationState.ValueRO.Value != currentState;
            }
        }
        
        [BurstCompile]
        private void DiscoverNpcAnimationState(ref SystemState state)
        {
            foreach (var animState in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithAll<NpcTag>())
            {
                var currentState = animState.ValueRO.Value;
            }
        }
    }
}