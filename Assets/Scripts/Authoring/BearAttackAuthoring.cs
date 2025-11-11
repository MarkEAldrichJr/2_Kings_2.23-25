using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BearAttackAuthoring : MonoBehaviour
    {
        [Header("Timers")]
        [SerializeField] private double startTime = 0.5d;
        [SerializeField] private double attackCooldownTime = 2d;
        [SerializeField] private double stopDamageTime = 1d;
        
        [Header("Dimensions")]
        [SerializeField] private float distanceForward = 2f;
        [SerializeField] private float radius = 2f;
        private class BearAttackAuthoringBaker : Baker<BearAttackAuthoring>
        {
            public override void Bake(BearAttackAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(e, new BearAttack
                {
                    StartTime = authoring.startTime,
                    DistanceForward = authoring.distanceForward,
                    CooldownTime = authoring.attackCooldownTime,
                    Radius = authoring.radius,
                    FrameCooldownFinishes = 0d,
                    StopDamageTime = authoring.stopDamageTime,
                    FrameStopDamage = 0d
                });
            }
        }
    }
}