using Authoring;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial struct FollowPlayerSystem : ISystem
    {
        private EntityQuery _bearQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _bearQuery =
                state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<BearTag, LocalTransform>());
            
            state.RequireForUpdate(_bearQuery);
            state.RequireForUpdate<AgentBody>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bears = _bearQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            if (bears.Length == 0) return;
            
            foreach (var (body, trans) in SystemAPI
                         .Query<RefRW<AgentBody>, RefRO<LocalTransform>>())
            {
                body.ValueRW.Destination = bears[0].Position;

                var dist = math.distancesq(body.ValueRO.Destination, trans.ValueRO.Position);

                if (dist > 2f)
                {
                    body.ValueRW.IsStopped = false;
                }
            }
        }
    }
}