using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ObstacleAuthoring : MonoBehaviour
    {
        private class ObstacleBaker : Baker<ObstacleAuthoring>
        {
            public override void Bake(ObstacleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<ObstacleTag>(entity);
            }
        }
    }

    public struct ObstacleTag: IComponentData { }
}