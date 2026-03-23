using Authoring.Child;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BearAttackSystem))]
    public partial struct KnockbackSystem : ISystem
    {
        private EntityQuery _knockbackQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<KnockbackRequest, KinematicCharacterBody>();
            _knockbackQuery = state.GetEntityQuery(builder);
            
            state.RequireForUpdate(_knockbackQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (knockbackRequest, body) in SystemAPI
                         .Query<KnockbackRequest, RefRW<KinematicCharacterBody>>())
            {
                body.ValueRW.RelativeVelocity +=
                    knockbackRequest.Direction * knockbackRequest.Force;
            }
            
            state.EntityManager.RemoveComponent<KnockbackRequest>(_knockbackQuery);
        }
    }
}