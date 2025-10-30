using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class EntityTransformFollower : MonoBehaviour
    {
        
        private EntityManager _entityManager;

        private void OnEnable()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            /*
             _entityManager.SetComponentData(Entity, new LocalTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale.x
            });
            */
        }

        private void LateUpdate()
        {
            /*
             if (Entity == Entity.Null) return;
            
            var entityTrans = 
                _entityManager.GetComponentData<LocalTransform>(Entity);

            transform.position = entityTrans.Position;
            transform.rotation = entityTrans.Rotation;
            
            var scale = entityTrans.Scale;
            transform.localScale = Vector3.one * scale;
            */
        }
    }
}