using Component;
using Component.NPCs;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Behavior
{
    public partial struct MoveToTargetSystem : ISystem
    {
        private EntityQuery _elishaQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, MoveToTargetFlag>();
            state.RequireForUpdate(state.GetEntityQuery(builder));

            var elishaBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ElishaFaith, LocalTransform, AnimationStateComp>()
                .WithNone<RequirePlayerTag>();
            _elishaQuery = state.GetEntityQuery(elishaBuilder);
            state.RequireForUpdate(_elishaQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var elishaTransforms = _elishaQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var elishaPos = elishaTransforms[0].Position;
            elishaTransforms.Dispose();
            
            foreach (var body in SystemAPI
                         .Query<RefRW<AgentBody>>()
                         .WithAll<MoveToTargetFlag>())
            {
                var dist = math.distancesq(body.ValueRO.Destination, elishaPos);
                if (dist < 0.2f) continue;
                
                body.ValueRW.SetDestination(elishaPos);
            }
        }
    }
}