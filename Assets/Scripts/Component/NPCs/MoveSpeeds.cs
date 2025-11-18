using Unity.Entities;

namespace Component.NPCs
{
    public struct MoveSpeeds : IComponentData
    {
        public float RunSpeed;
        public float WalkSpeed;
    }
}