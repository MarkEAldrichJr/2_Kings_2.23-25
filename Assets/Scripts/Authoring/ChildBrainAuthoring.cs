using Component;
using Component.NPCs;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ChildBrainAuthoring : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float attackRadius = 5f;
        
        public class ChildBrainBaker : Baker<ChildBrainAuthoring>
        {
            public override void Bake(ChildBrainAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                
                AddComponent<MoveToTargetFlag>(entity);
                AddComponent<AttackFlag>(entity);
                AddComponent<FleeFlag>(entity);
                AddComponent<SneakFlag>(entity);
                AddComponent(entity, new Detection
                {
                    DetectionRadius = authoring.detectionRadius,
                    AttackRadius = authoring.attackRadius
                });
                
                SetComponentEnabled<AttackFlag>(entity, false);
                SetComponentEnabled<SneakFlag>(entity, false);
                SetComponentEnabled<FleeFlag>(entity, false);
            }
        }
    }
}

//Run toward Elisha
// When withing range, mock him
//when bears nearby, run away
//Extra: When bears nearby but prone, sneak