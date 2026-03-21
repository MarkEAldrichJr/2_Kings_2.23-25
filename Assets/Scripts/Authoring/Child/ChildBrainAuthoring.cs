using Component.NPCs;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ChildBrainAuthoring : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float attackRadius = 5f;
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float runSpeed = 3.5f;

        [SerializeField] private float attackDamage = 1f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        
        public class ChildBrainBaker : Baker<ChildBrainAuthoring>
        {
            public override void Bake(ChildBrainAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                
                AddComponent<MoveToTargetFlag>(entity);
                AddComponent(entity, new AttackFlag
                {
                    AttackDamage = authoring.attackDamage,
                    TimeSinceLastAttack = 0f,
                    TimeBetweenAttacks = authoring.timeBetweenAttacks
                });
                AddComponent<FleeFlag>(entity);
                AddComponent<StartFleeFlag>(entity);
                AddComponent<SneakFlag>(entity);
                AddComponent(entity, new Detection
                {
                    DetectionRadius = authoring.detectionRadius,
                    AttackRadius = authoring.attackRadius
                });
                AddComponent(entity, new MoveSpeeds
                {
                    WalkSpeed = authoring.walkSpeed,
                    RunSpeed = authoring.runSpeed
                });
                
                SetComponentEnabled<AttackFlag>(entity, false);
                SetComponentEnabled<SneakFlag>(entity, false);
                SetComponentEnabled<FleeFlag>(entity, false);
                SetComponentEnabled<StartFleeFlag>(entity, false);
            }
        }
    }
}