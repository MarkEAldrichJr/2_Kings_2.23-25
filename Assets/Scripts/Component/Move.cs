using Unity.Entities;
using Unity.Mathematics;

namespace Component
{
    public struct Move : IComponentData
    {
        public float3 Direction;
        public float Speed;
    }
}