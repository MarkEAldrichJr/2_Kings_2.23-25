using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Child
{
    public class KnockbackAuthoring : MonoBehaviour
    {
        public float knockbackForce = 5f;
        
        private class KnockbackBaker : Baker<KnockbackAuthoring>
        {
            public override void Bake(KnockbackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Knockback
                {
                    Force = authoring.knockbackForce
                });
            }
        }
    }

    public struct Knockback : IComponentData
    {
        public float Force;
    }

    public struct KnockbackRequest : IComponentData
    {
        public float3 Direction;
        public float Force;
    }
}