using Component.NPCs;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Behavior
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FleeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FleeFlag, LocalTransform, AgentBody>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, body) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRW<AgentBody>>()
                         .WithAll<FleeFlag>())
            {
                //get a random point on the navmesh within a distance of the entity
            }
            //pick random destination on navmesh within a distance
            //when within range of destination, pick another random destination
        }
    }
    
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(FleeSystem))]
    public partial struct StartFleeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FleeFlag, StartFleeFlag, LocalTransform, AgentBody>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, body, entity) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRW<AgentBody>>()
                         .WithAll<FleeFlag, StartFleeFlag>().WithEntityAccess())
            {
                state.EntityManager.SetComponentEnabled<StartFleeFlag>(entity, false);
                
                //get a random location on the navmesh
                //set agentbody to that location
            }
        }
    }
}