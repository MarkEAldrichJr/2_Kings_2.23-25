using Unity.Entities;

namespace Component
{
    public struct AnimationStateComp : IComponentData
    {
        public AnimationStateEnum Value;
        public bool HasChangedThisFrame;
        public float IdleWalkThreshold;
        public float WalkRunThreshold;
    }

    public enum AnimationStateEnum
    {
        Idle, Walk, Run, Jump, Prone, Attack, Fear
    }
}