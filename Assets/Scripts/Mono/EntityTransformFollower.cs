using Component;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class EntityTransformFollower : MonoBehaviour
    {
        public Entity Entity;
        
        private EntityManager _entityManager;
        private EntityArchetype _archetype;

        private void OnEnable()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _archetype = _entityManager.CreateArchetype( typeof(LocalTransform), typeof(Move));
            Entity = _entityManager.CreateEntity(_archetype);
            _entityManager.SetComponentData(Entity, new LocalTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale.x
            });
        }

        private void OnDisable()
        {
            _entityManager.DestroyEntity(Entity);
        }

        private void LateUpdate()
        {
            if (Entity == Entity.Null) return;
            
            var entityTrans = 
                _entityManager.GetComponentData<LocalTransform>(Entity);

            transform.position = entityTrans.Position;
            transform.rotation = entityTrans.Rotation;
            
            var scale = entityTrans.Scale;
            transform.localScale = Vector3.one * scale;
        }
    }
}