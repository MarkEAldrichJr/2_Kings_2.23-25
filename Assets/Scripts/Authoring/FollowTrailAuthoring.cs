using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FollowTrailAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject firstPoint;
        
        public class FollowTrailBaker : Baker<FollowTrailAuthoring>
        {
            public override void Bake(FollowTrailAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new FollowTrail
                {
                    Target = GetEntity(authoring.firstPoint, TransformUsageFlags.Renderable)
                });
                AddComponent<InitializeTag>(entity);
            }
        }
    }

    public struct FollowTrail : IComponentData
    {
        public Entity Target;
    }
}