using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BearBaker : MonoBehaviour
    {
        private class BearBakerBaker : Baker<BearBaker>
        {
            public override void Bake(BearBaker authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                
                AddComponent<BearTag>(entity);
            }
        }
    }

    public struct BearTag : IComponentData { }
}