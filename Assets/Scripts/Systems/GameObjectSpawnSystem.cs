using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameObjectSpawnSystem : SystemBase
    {
        private EntityManager _entityManager;
        private EntityQuery _entityQuery;
        
        protected override void OnCreate()
        {
            _entityManager = World.EntityManager;
            _entityQuery = EntityManager.CreateEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<AnimationGameObjectPrefab, LocalTransform>());
            
            RequireForUpdate(_entityQuery);
        }

        protected override void OnUpdate()
        {
            /*foreach (var (prefab, transform, entity) in SystemAPI
                         .Query<RefRO<AnimationGameObjectPrefab>, RefRO<LocalTransform>>()
                         .WithEntityAccess())
            {
                var newBody = (GameObject)Object.Instantiate(prefab.ValueRO.Prefab,
                    transform.ValueRO.Position,
                    transform.ValueRO.Rotation);
                
                _entityManager.AddComponentData(entity, new AnimatorRefComponent
                {
                    AnimatorGo = newBody.GetComponent<Animator>(),
                    TransGo = newBody.transform
                });
            }*/
            
            var entities = _entityQuery.ToEntityArray(Allocator.Temp);
            var prefabs = _entityQuery.ToComponentDataArray<AnimationGameObjectPrefab>(Allocator.Temp);
            var transforms = _entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            for (var i = 0; i < entities.Length; i++)
            {
                var newBody = (GameObject)Object.Instantiate(prefabs[i].Prefab,
                    transforms[i].Position, transforms[i].Rotation);
                
                _entityManager.AddComponentData(entities[i], new AnimatorRefComponent
                {
                    AnimatorGo = newBody.GetComponent<Animator>(),
                    TransGo = newBody.transform
                });
            }
            
            _entityManager.RemoveComponent<AnimationGameObjectPrefab>(_entityQuery);

            entities.Dispose();
            transforms.Dispose();
            prefabs.Dispose();
        }
    }

    public struct AnimatorRefComponent : IComponentData
    {
        public UnityObjectRef<Transform> TransGo;
        public UnityObjectRef<Animator>  AnimatorGo;
    }
}