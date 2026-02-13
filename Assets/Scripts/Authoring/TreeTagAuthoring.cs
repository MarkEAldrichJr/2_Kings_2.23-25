using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TreeTagAuthoring : MonoBehaviour
    {
        private class TreeTagAuthoringBaker : Baker<TreeTagAuthoring>
        {
            public override void Bake(TreeTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<TreeTag>(entity);
            }
        }
    }
}