using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class DeathByBearAuthoring : MonoBehaviour
    {
        private class DeathByBearAuthoringBaker : Baker<DeathByBearAuthoring>
        {
            public override void Bake(DeathByBearAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<DeathByBearTag>(e);
            }
        }
    }
}