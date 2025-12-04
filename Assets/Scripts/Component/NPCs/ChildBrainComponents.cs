using Unity.Entities;

namespace Component.NPCs
{
    public struct MoveToTargetFlag : IComponentData, IEnableableComponent {}

    public struct AttackFlag : IComponentData, IEnableableComponent
    {
        public float TimeSinceLastAttack;
        public float AttackDamage;
        public float TimeBetweenAttacks;
    }
    public struct FleeFlag : IComponentData, IEnableableComponent {}
    public struct StartFleeFlag : IComponentData, IEnableableComponent {}
    public struct SneakFlag : IComponentData, IEnableableComponent {}
    public struct Detection : IComponentData
    {
        public float DetectionRadius;
        public float AttackRadius;
    }
}