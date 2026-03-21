using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Child
{
    public class ChildAuthoring : MonoBehaviour
    {
        private class ChildBaker : Baker<ChildAuthoring>
        {
            public override void Bake(ChildAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<ChildTag>(e);
            }
        }
    }
}