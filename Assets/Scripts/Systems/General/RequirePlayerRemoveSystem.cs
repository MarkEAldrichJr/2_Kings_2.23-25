using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.General
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct RequirePlayerRemoveSystem : ISystem
    {
        private EntityQuery _query;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp).
                    WithAll<RequirePlayerTag>());
            state.RequireForUpdate(_query);
            
            //gate to keep from running without a player present
            state.RequireForUpdate<ThirdPersonCharacterComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.RemoveComponent<RequirePlayerTag>(_query);
        }
    }
}