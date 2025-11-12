using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class AnimationPrefabAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        
        public class AnimationBaker : Baker<AnimationPrefabAuthoring>
        {
            public override void Bake(AnimationPrefabAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                
                AddComponent(entity, new AnimationGameObjectPrefab
                {
                    Prefab = authoring.prefab
                });
                
                AddComponent<AnimationStateComp>(entity);
            }
        }
    }

    public struct AnimationGameObjectPrefab : IComponentData
    {
        public UnityObjectRef<GameObject> Prefab;
    }
}