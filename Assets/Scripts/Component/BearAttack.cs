using Unity.Entities;

namespace Component
{
    public struct BearAttack : IComponentData
    {
        public double AnimationDelay; //start damaging AFTER windup
        public double FrameToHit;
        
        public double CooldownTime;
        public double FrameCooldownFinishes;

        public bool HasHit;
        
        public float DistanceForward;
        public float Radius;
    }
    
    public struct ChildTag: IComponentData { }
}