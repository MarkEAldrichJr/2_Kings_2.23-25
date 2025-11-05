using Unity.Entities;

namespace Component
{
    public struct AnimationState : IComponentData
    {
        public AnimationStateEnum Value;
        public bool HasChangedThisFrame;
    }

    public enum AnimationStateEnum
    {
        Idle, Walk, Run, Jump, Sleep, Crouch, Prone
    }
}