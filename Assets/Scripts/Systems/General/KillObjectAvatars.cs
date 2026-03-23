using System.Collections.Generic;
using Component;
using Component.NPCs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems.General
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(KillEntities))]
    public partial class KillObjectAvatars : SystemBase
    {
        protected override void OnUpdate()
        {
            var killedGameObjects = new List<GameObject>();
            
            foreach (var animatorRef in SystemAPI
                         .Query<RefRO<AnimatorRefComponent>>()
                         .WithAll<KillTag>())
            {
                var animator = animatorRef.ValueRO.AnimatorGo.Value.gameObject;
                killedGameObjects.Add(animator);
            }

            for (var i = 0; i < killedGameObjects.Count; i++)
            {
                Object.Destroy(killedGameObjects[i]);
            }
            killedGameObjects.Clear();
        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(KillObjectAvatars))]
    public partial struct KillEntities : ISystem
    {
        private EntityQuery _killQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<KillTag>();
            _killQuery = state.GetEntityQuery(builder);
            
            state.RequireForUpdate<KillTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.DestroyEntity(_killQuery);
        }
    }
}