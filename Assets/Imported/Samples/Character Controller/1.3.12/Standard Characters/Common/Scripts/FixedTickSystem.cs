using Unity.Burst;
using Unity.Entities;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct FixedTickSystem : ISystem
    {
        public struct FixedTickSingleton : IComponentData
        {
            public uint Tick;
        }

        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<FixedTickSingleton>())
            {
                var singletonEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(singletonEntity, new FixedTickSingleton());
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var singleton = ref SystemAPI.GetSingletonRW<FixedTickSingleton>().ValueRW;
            singleton.Tick++;
        }
    }
}
