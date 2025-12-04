using Unity.Entities;

namespace Component
{
    public struct ElishaFaith : IComponentData
    {
        public float FaithMax;
        public float CurrentFaith;
        public float FaithRegen;
        public float TimeSinceLastDamage;
    }

    public struct FaithDamageElement : IBufferElementData
    {
        public float Damage;
    }
}