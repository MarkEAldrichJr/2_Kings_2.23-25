using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Authoring
{
    public class RandomizeTreeAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector3 tiltDirectionDelta;
        [SerializeField] private float scaleDelta;
        [SerializeField] private float heightDelta;
        
        public class RandomizeTreeBaker : Baker<RandomizeTreeAuthoring>
        {
            public override void Bake(RandomizeTreeAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Renderable);
                AddComponent(e, new RandomPosChange
                {
                    HeightDelta = authoring.heightDelta,
                    ScaleDelta = authoring.scaleDelta,
                    TiltDirectionsDelta = authoring.tiltDirectionDelta
                });
                AddComponent(e, new RandomComp
                {
                    Value = Random.CreateFromIndex((uint)authoring.gameObject.GetInstanceID())
                });
            }
        }
    }

    public struct RandomPosChange : IComponentData
    {
        public float3 TiltDirectionsDelta;
        public float ScaleDelta;
        public float HeightDelta;
    }

    public struct RandomComp : IComponentData
    {
        public Random Value;
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct MoveRandomAtStartSystem : ISystem
    {
        private EntityQuery _query;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RandomPosChange, LocalTransform>();

            _query = state.GetEntityQuery(builder);
            
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (pos, rand, trans) in SystemAPI
                         .Query<RefRO<RandomPosChange>, RefRW<RandomComp>, RefRW<LocalTransform>>())
            {
                var tiltDelta = pos.ValueRO.TiltDirectionsDelta;
                var scaleDelta = pos.ValueRO.ScaleDelta;
                var heightDelta = pos.ValueRO.HeightDelta;
                var newTiltDelta = float3.zero;
                
                newTiltDelta.x = rand.ValueRW.Value.NextFloat(-tiltDelta.x, tiltDelta.x);
                newTiltDelta.y = rand.ValueRW.Value.NextFloat(-tiltDelta.y, tiltDelta.y);
                newTiltDelta.z = rand.ValueRW.Value.NextFloat(-tiltDelta.z, tiltDelta.z);

                trans.ValueRW.Scale = rand.ValueRW.Value.NextFloat(1 - scaleDelta, 1 + scaleDelta);
                trans.ValueRW.Position.y = rand.ValueRW.Value.NextFloat(
                    trans.ValueRO.Position.y - 1 - heightDelta,
                    trans.ValueRO.Position.y - 1);
                trans.ValueRW.Rotation = quaternion.EulerXYZ(math.radians(newTiltDelta));
            }
            state.EntityManager.RemoveComponent<RandomPosChange>(_query);
        }
    }
}