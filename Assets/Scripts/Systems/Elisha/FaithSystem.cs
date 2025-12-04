using Component;
using Systems.Behavior;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Elisha
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ChildBrainDeciderSystem))]
    public partial struct FaithSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ElishaFaith>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (faith, damage) in SystemAPI
                         .Query<RefRW<ElishaFaith>, DynamicBuffer<FaithDamageElement>>())
            {
                if (damage.Length > 0f)
                    faith.ValueRW.TimeSinceLastDamage = 0f;
                faith.ValueRW.TimeSinceLastDamage += deltaTime;
                
                var totalDamage = 0f;
                foreach (var dmg in damage)
                    totalDamage += dmg.Damage;
                
                var heal = faith.ValueRO.CurrentFaith * faith.ValueRO.FaithRegen * deltaTime;
                var newFaith = faith.ValueRO.CurrentFaith + heal - totalDamage;
                faith.ValueRW.CurrentFaith = newFaith;

                faith.ValueRW.CurrentFaith = math.clamp(faith.ValueRO.CurrentFaith, -1f,
                    faith.ValueRO.FaithMax);

                if (faith.ValueRO.CurrentFaith < 0f)
                {
                    var gameOverEntity = state.EntityManager.CreateEntity();
                    state.EntityManager.AddComponent<GameOverTag>(gameOverEntity);
                }
            }
        }
    }

    public struct GameOverTag : IComponentData { }
}