using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BearAttackAuthoring : MonoBehaviour
    {
        [SerializeField] private float distanceForward;
        [SerializeField] private uint attackCooldownTime;
        [SerializeField] private float radius;
        [SerializeField] private uint stopDamageTime;
        
        private class BearAttackAuthoringBaker : Baker<BearAttackAuthoring>
        {
            public override void Bake(BearAttackAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(e, new BearAttack
                {
                    DistanceForward = authoring.distanceForward,
                    CooldownTime = authoring.attackCooldownTime,
                    Radius = authoring.radius,
                    FrameCooldownFinishes = 0u,
                    StopDamageTime = authoring.stopDamageTime,
                    FrameStopDamage = 0u
                });
            }
        }
    }
}