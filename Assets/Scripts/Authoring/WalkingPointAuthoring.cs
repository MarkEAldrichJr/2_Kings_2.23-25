using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class WalkingPointAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject nextPoint;
        
        public class WalkingPointBaker : Baker<WalkingPointAuthoring>
        {
            public override void Bake(WalkingPointAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Renderable);
                AddComponent(entity, new WalkingPoint
                {
                    NextPoint = GetEntity(authoring.nextPoint, TransformUsageFlags.Renderable)
                });
            }
        }
    }

    public struct WalkingPoint : IComponentData
    {
        public Entity NextPoint;
    }
}