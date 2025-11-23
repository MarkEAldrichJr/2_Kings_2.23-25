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
            var endgame = false;
            foreach (var faith in SystemAPI.Query<RefRW<ElishaFaith>>())
            {
                var dmg = faith.ValueRO.NumChildren * faith.ValueRO.DamagePerChild * deltaTime;
                var heal = faith.ValueRO.CurrentFaith * faith.ValueRO.FaithRegen * deltaTime;
                var newFaith = faith.ValueRO.CurrentFaith + heal - dmg;
                faith.ValueRW.CurrentFaith = newFaith;

                faith.ValueRW.CurrentFaith = math.clamp(faith.ValueRO.CurrentFaith, -1f,
                    faith.ValueRO.FaithMax);

                if (faith.ValueRO.CurrentFaith < 0f)
                {
                    endgame = true;
                }
            }

            if (endgame)
            { 
                var gameOverEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponent<GameOverTag>(gameOverEntity);
            }
        }
    }

    public struct GameOverTag : IComponentData { }
}