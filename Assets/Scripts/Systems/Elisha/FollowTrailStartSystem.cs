using Authoring;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Elisha
{
    public partial struct FollowTrailStartSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (agentBody, trail) in SystemAPI
                         .Query<RefRW<AgentBody>, RefRO<FollowTrail>>()
                         .WithNone<RequirePlayerTag>())
            {
                var firstDest = state.EntityManager
                    .GetComponentData<LocalToWorld>(trail.ValueRO.Target).Position;
                agentBody.ValueRW.SetDestination(firstDest);
                agentBody.ValueRW.IsStopped = false;
            }
        }
    }
}