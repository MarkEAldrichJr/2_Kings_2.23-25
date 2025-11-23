using Component;
using Component.NPCs;
using Systems.Elisha;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    public partial struct GameOverKillEveryoneSystem : ISystem
    {
        private EntityQuery _query;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimatorRefComponent>()
                .WithNone<KillTag>();
            _query = state.GetEntityQuery(builder);
            state.RequireForUpdate(_query);
            state.RequireForUpdate<GameOverTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<KillTag>(_query);
        }
    }
}