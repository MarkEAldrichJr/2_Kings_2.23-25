using Component;
using Component.NPCs;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Behavior
{
    public partial struct AttackTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ElishaFaith>();

            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AgentBody, AttackFlag>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageBuffer = SystemAPI.GetSingletonBuffer<FaithDamageElement>();
            
            
            foreach (var (body, attack) in SystemAPI
                         .Query<RefRW<AgentBody>, RefRW<AttackFlag>>())
            {
                body.ValueRW.IsStopped = true;
                if (attack.ValueRO.TimeSinceLastAttack > attack.ValueRO.TimeBetweenAttacks)
                {
                    damageBuffer.Add(new FaithDamageElement
                    {
                        Damage = attack.ValueRO.AttackDamage
                    });
                    attack.ValueRW.TimeSinceLastAttack = 0f;
                }
            }
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var attack in SystemAPI
                         .Query<RefRW<AttackFlag>>()
                         .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                attack.ValueRW.TimeSinceLastAttack += deltaTime;
            }
        }
    }
}