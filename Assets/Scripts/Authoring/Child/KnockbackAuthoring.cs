using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Child
{
    public class KnockbackAuthoring : MonoBehaviour
    {
        public float knockbackForce = 5f;
        public float launchAngle = 45f;
        
        private class KnockbackBaker : Baker<KnockbackAuthoring>
        {
            public override void Bake(KnockbackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Knockback
                {
                    Force = authoring.knockbackForce,
                    LaunchAngle = authoring.launchAngle,
                });
            }
        }
    }

    public struct Knockback : IComponentData
    {
        public float Force;
        public float LaunchAngle;
    }

    public struct KnockbackRequest : IComponentData
    {
        public float3 Direction;
        public float Force;
    }
}