using Unity.Burst;
using Unity.Entities;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct FixedTickSystem : ISystem
    {
        public struct Singleton : IComponentData
        {
            public uint Tick;
        }

        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Singleton>())
            {
                var singletonEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(singletonEntity, new Singleton());
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var singleton = ref SystemAPI.GetSingletonRW<Singleton>().ValueRW;
            singleton.Tick++;
        }
    }
}
