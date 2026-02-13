using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct ChangeTreeColorsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Simulate, TreeTag>();
            
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var random = new Random(12345);

            var leafGreen = new float4(0.18f, 0.45f, 0.15f, 1f);
            var leafOrange = new float4(0.85f, 0.42f, 0.08f, 1f);
            var barkBrown = new float4(0.36f, 0.26f, 0.18f, 1f);
            var barkGrey = new float4(0.45f, 0.45f, 0.42f, 1f);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var childBuffer in SystemAPI
                         .Query<DynamicBuffer<Child>>()
                         .WithAll<Simulate, TreeTag>())
            {
                var leafEntity = childBuffer[0].Value;
                var barkEntity = childBuffer[1].Value;
                
                var tBark = random.NextFloat();
                var tLeaf = random.NextFloat();
                
                ecb.AddComponent(barkEntity, new URPMaterialPropertyBaseColor
                {
                    Value = math.lerp(barkBrown, barkGrey, tBark)
                });
                ecb.AddComponent(leafEntity, new URPMaterialPropertyBaseColor
                {
                    Value = math.lerp(leafGreen, leafOrange, tLeaf)
                });
            }
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}