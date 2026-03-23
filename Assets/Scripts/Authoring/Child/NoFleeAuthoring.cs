using Unity.Entities;
using UnityEngine;

namespace Authoring.Child
{
    public class NoFleeAuthoring : MonoBehaviour
    {
        private class NoFleeBaker : Baker<NoFleeAuthoring>
        {
            public override void Bake(NoFleeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<NoFleeTag>(entity);
            }
        }
    }
    
    public struct NoFleeTag : IComponentData { }
}