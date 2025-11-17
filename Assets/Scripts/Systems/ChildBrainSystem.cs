using Authoring;
using Component;
using Component.NPCs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    /// <summary>
    /// Changes Child state based on distance from players and Elisha
    /// Other Systems handle behavior in each state
    /// </summary>
    public partial struct ChildBrainDeciderSystem : ISystem
    {
        private EntityQuery _childQuery;
        private EntityQuery _bearQuery;
        private EntityQuery _elishaQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ElishaFaith>();
            
            var childBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<DeathByBearTag>()
                .WithAny<SneakFlag, FleeFlag, AttackFlag, MoveToTargetFlag>();
            _childQuery = state.GetEntityQuery(childBuilder);
            state.RequireForUpdate(_childQuery);
            
            var elishaBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FollowTrail, LocalTransform, ElishaFaith>()
                .WithNone<RequirePlayerTag>();
            _elishaQuery = state.GetEntityQuery(elishaBuilder);
            state.RequireForUpdate(_elishaQuery);

            var bearBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BearTag, LocalTransform, AnimationStateComp>();
            _bearQuery = state.GetEntityQuery(bearBuilder);
            state.RequireForUpdate(_bearQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bearTransforms = _bearQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var bearAnimationState = _bearQuery.ToComponentDataArray<AnimationStateComp>(Allocator.Temp);

            var elishaEntity = SystemAPI.GetSingletonEntity<ElishaFaith>();
            var elishaTransform = state.EntityManager.GetComponentData<LocalTransform>(elishaEntity);
            var elishaFaith = state.EntityManager.GetComponentData<ElishaFaith>(elishaEntity);
            
            
            foreach (var (detection, localTransform, entity) in SystemAPI
                         .Query<RefRO<Detection>, RefRO<LocalTransform>>()
                         .WithAll<DeathByBearTag, MoveToTargetFlag>()
                         .WithEntityAccess())
            {
                var bearIsClose = false;
                for (var i = 0; i < bearTransforms.Length; i++)
                {
                    var distToBear = math.distance(bearTransforms[i].Position,
                        localTransform.ValueRO.Position);
                    if (distToBear < detection.ValueRO.DetectionRadius)
                    {
                        bearIsClose = true;
                        state.EntityManager.SetComponentEnabled<MoveToTargetFlag>(entity, false);
                        if (bearAnimationState[i].Value == AnimationStateEnum.Prone)
                        {
                            state.EntityManager.SetComponentEnabled<SneakFlag>(entity, true);
                        }
                        else
                        {
                            state.EntityManager.SetComponentEnabled<FleeFlag>(entity, true);
                        }
                    }
                }

                if (bearIsClose) continue;

                var distToElisha = math.distance(localTransform.ValueRO.Position,
                    elishaTransform.Position);
                if (distToElisha < detection.ValueRO.AttackRadius)
                {
                    state.EntityManager.SetComponentEnabled<MoveToTargetFlag>(entity, false);
                    StartAttack(ref state, entity, elishaEntity, elishaFaith);
                }
            }

            foreach (var (detection, localTransform, entity) in SystemAPI
                         .Query<RefRO<Detection>, RefRO<LocalTransform>>()
                         .WithAll<DeathByBearTag, AttackFlag>()
                         .WithEntityAccess())
            {
                var distToElisha = math.distance(elishaTransform.Position, localTransform.ValueRO.Position);
                if (distToElisha > detection.ValueRO.DetectionRadius)
                {
                    state.EntityManager.SetComponentEnabled<MoveToTargetFlag>(entity, true);
                    EndAttack(ref state, entity, elishaEntity, elishaFaith);
                    continue;
                }

                var bearIsClose = false;
                for (var i = 0; i < bearTransforms.Length; i++)
                {
                    if (bearAnimationState[i].Value == AnimationStateEnum.Prone) continue;
                    var distToBear = math.distance(bearTransforms[i].Position, localTransform.ValueRO.Position);
                    if (distToBear < detection.ValueRO.DetectionRadius)
                        bearIsClose = true;
                }
                if (!bearIsClose) continue;
                
                state.EntityManager.SetComponentEnabled<FleeFlag>(entity, true);
                EndAttack(ref state, entity, elishaEntity, elishaFaith);
            }

            foreach (var (detection, localTransform, entity) in SystemAPI
                         .Query<RefRO<Detection>, RefRO<LocalTransform>>()
                         .WithAll<DeathByBearTag, FleeFlag>()
                         .WithEntityAccess())
            {
                //check distance to player.  if outside of detection range, change back to MoveTotTargetFlag
                var bearInRange = false;
                for (var i = 0; i < bearTransforms.Length; i++)
                {
                    var distToBear = math.distance(localTransform.ValueRO.Position, bearTransforms[i].Position);
                    if (distToBear < detection.ValueRO.DetectionRadius)
                        bearInRange = true;
                }

                if (bearInRange) continue;
                state.EntityManager.SetComponentEnabled<MoveToTargetFlag>(entity, true);
                state.EntityManager.SetComponentEnabled<FleeFlag>(entity, false);
            }
            
            foreach (var (detection, localTransform, entity) in SystemAPI
                         .Query<RefRO<Detection>, RefRO<LocalTransform>>()
                         .WithAll<DeathByBearTag, SneakFlag>()
                         .WithEntityAccess())
            {
                var dangerIsClose = false;
                var bearIsClose = false;
                for (var i = 0; i < bearTransforms.Length; i++)
                {
                    var distToBear = math.distance(localTransform.ValueRO.Position, bearTransforms[i].Position);
                    if (distToBear < detection.ValueRO.DetectionRadius)
                    {
                        bearIsClose = true;
                        if (bearAnimationState[i].Value != AnimationStateEnum.Prone)
                            dangerIsClose = true;
                    }
                }

                if (dangerIsClose)
                {
                    state.EntityManager.SetComponentEnabled<FleeFlag>(entity, true);
                    state.EntityManager.SetComponentEnabled<SneakFlag>(entity, false);
                    continue;
                }
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (bearIsClose && !dangerIsClose) continue;
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!bearIsClose && !dangerIsClose)
                {
                    //if within range of elisha, start mocking
                    var distToElisha = math.distance(localTransform.ValueRO.Position, elishaTransform.Position);
                    if (distToElisha < detection.ValueRO.AttackRadius)
                    {
                        StartAttack(ref state, entity, elishaEntity, elishaFaith);
                        state.EntityManager.SetComponentEnabled<SneakFlag>(entity, false);
                    }
                    else
                    {
                        state.EntityManager.SetComponentEnabled<MoveToTargetFlag>(entity, true);
                        state.EntityManager.SetComponentEnabled<SneakFlag>(entity, false);
                    }
                }
            }
            
            bearTransforms.Dispose();
            bearAnimationState.Dispose();
        }

        [BurstCompile]
        private static void StartAttack(ref SystemState state, Entity child,
            Entity elisha, ElishaFaith faith)
        {
            state.EntityManager.SetComponentEnabled<AttackFlag>(child, true);
            faith.NumChildren++;
            state.EntityManager.SetComponentData(elisha, faith);
        }

        [BurstCompile]
        private static void EndAttack(ref SystemState state, Entity child,
            Entity elisha, ElishaFaith faith)
        {
            state.EntityManager.SetComponentEnabled<AttackFlag>(child, false);
            faith.NumChildren--;
            state.EntityManager.SetComponentData(elisha, faith);
        }
    }
}