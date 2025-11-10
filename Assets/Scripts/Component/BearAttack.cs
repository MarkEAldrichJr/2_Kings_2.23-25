using Unity.Entities;

namespace Component
{
    public struct BearAttack : IComponentData
    {
        public float DistanceForward;
        public uint CooldownTime;
        public uint FrameCooldownFinishes;

        public uint StopDamageTime;
        public uint FrameStopDamage;
        
        public float Radius;
    }
    
    public struct DeathByBearTag: IComponentData { }
}