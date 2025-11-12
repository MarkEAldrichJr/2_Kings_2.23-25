using Component;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Animations
{
    [UpdateBefore(typeof(BearAttackSystem))]
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
            foreach (var (animationState, characterControl, characterBody, bearAttack) in SystemAPI
                         .Query<RefRW<AnimationStateComp>, RefRW<ThirdPersonCharacterControl>,
                             RefRO<KinematicCharacterBody>, RefRO<BearAttack>>())
            {
                var currentState = animationState.ValueRO.Value;
                var isGrounded = characterBody.ValueRO.IsGrounded;
                var velocity = math.length(characterBody.ValueRO.RelativeVelocity);
                
                if (characterControl.ValueRO.Attack && 
                    bearAttack.ValueRO.FrameCooldownFinishes <= SystemAPI.Time.ElapsedTime)
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