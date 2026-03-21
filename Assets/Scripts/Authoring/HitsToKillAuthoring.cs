using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HitsToKillAuthoring : MonoBehaviour
    {
        public int hitsToDestroy;
        private class HitsToKillBaker : Baker<HitsToKillAuthoring>
        {
            public override void Bake(HitsToKillAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new HitsToKill
                {
                    Value = authoring.hitsToDestroy,
                });
            }
        }
    }

    public struct HitsToKill : IComponentData
    {
        public int Value;
    }
}