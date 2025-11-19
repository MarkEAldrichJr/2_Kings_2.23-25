using Authoring;
using Component;
using Component.NPCs;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Animations
{
    [UpdateBefore(typeof(Player.BearAttackSystem))]
    public partial struct AnimationStateDiscoverySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var playerBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimationStateComp>();
            state.RequireForUpdate(state.GetEntityQuery(playerBuilder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            DiscoverPlayerAnimationState(ref state);
            DiscoverElishaAnimationState(ref state);
            DiscoverChildAnimationState(ref state);
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
        private void DiscoverElishaAnimationState(ref SystemState state)
        {
            foreach (var (animState, elishaFaith, body) in SystemAPI
                         .Query<RefRW<AnimationStateComp>, RefRO<ElishaFaith>, RefRW<AgentBody>>()
                         .WithAll<NpcTag, FollowTrail>().WithNone<RequirePlayerTag>())
            {
                var currentState = animState.ValueRO.Value;
                
                if (elishaFaith.ValueRO.NumChildren > 0)
                {
                    body.ValueRW.IsStopped = true;
                    animState.ValueRW.Value = AnimationStateEnum.Fear;
                }
                else
                {
                    body.ValueRW.IsStopped = false;
                    animState.ValueRW.Value = AnimationStateEnum.Walk;
                } //Elisha will not stand idle at any point after the game starts
                animState.ValueRW.HasChangedThisFrame = animState.ValueRO.Value != currentState;
            }
        }

        [BurstCompile]
        private void DiscoverChildAnimationState(ref SystemState state)
        {
            foreach (var animState in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithAll<FleeFlag>())
            {
                if (animState.ValueRO.Value == AnimationStateEnum.Fear) continue;

                animState.ValueRW.Value = AnimationStateEnum.Fear;
                animState.ValueRW.HasChangedThisFrame = true;
            }
            
            foreach (var animState in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithAll<MoveToTargetFlag>())
            {
                if (animState.ValueRO.Value == AnimationStateEnum.Run) continue;

                animState.ValueRW.Value = AnimationStateEnum.Run;
                animState.ValueRW.HasChangedThisFrame = true;
            }
            
            foreach (var animState in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithAll<SneakFlag>())
            {
                if (animState.ValueRO.Value == AnimationStateEnum.Walk) continue;

                animState.ValueRW.Value = AnimationStateEnum.Walk;
                animState.ValueRW.HasChangedThisFrame = true;
            }
            
            foreach (var animState in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithAll<AttackFlag>())
            {
                if (animState.ValueRO.Value == AnimationStateEnum.Attack) continue;

                animState.ValueRW.Value = AnimationStateEnum.Attack;
                animState.ValueRW.HasChangedThisFrame = true;
            }
        }
    }
}