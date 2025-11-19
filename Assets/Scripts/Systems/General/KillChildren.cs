using System.Collections.Generic;
using Component;
using Component.NPCs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems.General
{
    public partial class KillChildren : SystemBase
    {
        private EntityQuery _entityQuery;
        
        protected override void OnCreate()
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<AnimatorRefComponent, KillTag>();
            _entityQuery = GetEntityQuery(builder);
            
            RequireForUpdate<KillTag>();
        }

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
            
            EntityManager.DestroyEntity(_entityQuery);
            killedGameObjects.Clear();
        }
    }
}