using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct ChangeTreeColors : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Simulate, TreeTag, UrpMaterialPropertyBaseColor, UrpMaterialPropertyBaseColor1>();
            
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
            
            foreach (var (mat, mat1) in SystemAPI
                         .Query<RefRW<UrpMaterialPropertyBaseColor>, RefRW<UrpMaterialPropertyBaseColor1>>()
                         .WithAll<Simulate, TreeTag>())
            {
                var tBark = random.NextFloat();
                var tLeaf = random.NextFloat();
                
                mat.ValueRW.Value = math.lerp(leafGreen, leafOrange, tLeaf);
                mat1.ValueRW.Value = math.lerp(barkBrown, barkGrey, tBark);
            }
            
            state.Enabled = false;
        }
    }
}