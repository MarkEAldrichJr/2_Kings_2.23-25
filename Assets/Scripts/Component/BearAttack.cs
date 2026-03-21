using Unity.Entities;

namespace Component
{
    public struct BearAttack : IComponentData
    {
        public double StartTime; //start damaging AFTER windup
        public double FrameToStart;
        
        public double CooldownTime;
        public double FrameCooldownFinishes;

        public double StopDamageTime;
        public double FrameStopDamage;
        
        public float DistanceForward;
        public float Radius;
    }
    
    public struct ChildTag: IComponentData { }

    public struct BearAttacksToDestroy : IComponentData
    {
        public int BearAttacksNeeded;
    }
}