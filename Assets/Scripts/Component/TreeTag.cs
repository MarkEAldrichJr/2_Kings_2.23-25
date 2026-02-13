using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Component
{
    public struct TreeTag : IComponentData { }

    [MaterialProperty("_BaseMapColor", 1)]
    public struct UrpMaterialPropertyBaseColor1 : IComponentData
    {
        public float4 Value;
    }

    [MaterialProperty("_BaseMapColor", 0)]
    public struct UrpMaterialPropertyBaseColor : IComponentData
    {
        public float4 Value;
    }
}
