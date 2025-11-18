using Component.NPCs;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class NpcTagAuthoring : MonoBehaviour
    {
        private class NpcTagAuthoringBaker : Baker<NpcTagAuthoring>
        {
            public override void Bake(NpcTagAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent<NpcTag>(e);
            }
        }
    }
}