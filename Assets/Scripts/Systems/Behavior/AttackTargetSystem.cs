using Component.NPCs;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Behavior
{
    public partial struct AttackTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, AttackFlag>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var body in SystemAPI
                         .Query<RefRW<AgentBody>>()
                         .WithAll<AttackFlag>())
            {
                body.ValueRW.IsStopped = true;
            }
        }
    }
}