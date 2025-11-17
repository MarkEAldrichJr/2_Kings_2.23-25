using Unity.Entities;

namespace Component
{
    public struct MoveToTargetFlag : IComponentData, IEnableableComponent {}
    public struct AttackFlag : IComponentData, IEnableableComponent {}
    public struct FleeFlag : IComponentData, IEnableableComponent {}
    public struct SneakFlag : IComponentData, IEnableableComponent {}
    public struct Detection : IComponentData
    {
        public float DetectionRadius;
        public float AttackRadius;
    }
}