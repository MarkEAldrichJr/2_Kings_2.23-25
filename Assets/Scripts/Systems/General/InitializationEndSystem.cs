using Authoring;
using Unity.Collections;
using Unity.Entities;

namespace Systems.General
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct InitializationEndSystem : ISystem
    {
        private EntityQuery _entityQuery;
        public void OnCreate(ref SystemState state)
        {
            _entityQuery =
                state.GetEntityQuery(
                    new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<InitializeTag>());
            
            state.RequireForUpdate<InitializeTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.RemoveComponent<InitializeTag>(_entityQuery);
        }
    }
}