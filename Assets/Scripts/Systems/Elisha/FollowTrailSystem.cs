using Authoring;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FollowTrailSystem : ISystem
    {
        private EntityQuery _followTrailQuery;
        private EntityQuery _walkPointQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var followBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FollowTrail, LocalTransform>().WithNone<RequirePlayerTag>();
            _followTrailQuery = state.GetEntityQuery(followBuilder);

            var walkPointBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<WalkingPoint, LocalToWorld>();
            _walkPointQuery = state.GetEntityQuery(walkPointBuilder);
            
            state.RequireForUpdate(_followTrailQuery);
            state.RequireForUpdate(_walkPointQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (trail, trans, body) in SystemAPI
                         .Query<RefRW<FollowTrail>, RefRO<LocalTransform>, RefRW<AgentBody>>()
                         .WithNone<RequirePlayerTag>())
            {
                var targetPos =  state.EntityManager.GetComponentData<LocalToWorld>(trail.ValueRO.Target).Position;
                var dist = math.distancesq(targetPos, trans.ValueRO.Position);

                if (dist > 2) continue;
                
                var nextPos = state.EntityManager.GetComponentData<WalkingPoint>(trail.ValueRO.Target).NextPoint;
                trail.ValueRW.Target = nextPos;
                
                targetPos = state.EntityManager.GetComponentData<LocalToWorld>(trail.ValueRO.Target).Position;
                body.ValueRW.SetDestination(targetPos);
            }
        }
    }
}