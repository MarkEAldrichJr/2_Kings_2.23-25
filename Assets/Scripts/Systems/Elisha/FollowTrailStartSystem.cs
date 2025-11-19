using Authoring;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Elisha
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct FollowTrailStartSystem : ISystem
    {
        EntityQuery _followTrailStartQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FollowTrailStartTag, FollowTrail>()
                .WithNone<RequirePlayerTag>();
            _followTrailStartQuery = state.GetEntityQuery(builder);
            
            state.RequireForUpdate(_followTrailStartQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (agentBody, trail, e) in SystemAPI
                         .Query<RefRW<AgentBody>, RefRO<FollowTrail>>()
                         .WithAll<FollowTrailStartTag>()
                         .WithNone<RequirePlayerTag>()
                         .WithEntityAccess())
            {
                var firstDest = state.EntityManager
                    .GetComponentData<LocalToWorld>(trail.ValueRO.Target).Position;
                agentBody.ValueRW.SetDestination(firstDest);
                agentBody.ValueRW.IsStopped = false;
            }
            
            state.EntityManager.RemoveComponent<FollowTrailStartTag>(_followTrailStartQuery);
        }
    }
}