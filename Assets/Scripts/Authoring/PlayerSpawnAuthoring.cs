using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlayerSpawnAuthoring : MonoBehaviour
    {
        public class PlayerSpawnAuthoringBaker : Baker<PlayerSpawnAuthoring>
        {
            public override void Bake(PlayerSpawnAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Renderable);
                AddComponent<PlayerSpawnTag>(e);
            }
        }
    }
}