using Component;
using Component.NPCs;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;

namespace Systems.Behavior
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FleeSystem : ISystem
    {
        private Random _random;
        
        public void OnCreate(ref SystemState state)
        {
            _random = Random.CreateFromIndex((uint)System.DateTime.Now.Millisecond);
            
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FleeFlag, LocalTransform, AgentBody, DeathByBearTag>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, body) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRW<AgentBody>>()
                         .WithAll<FleeFlag, DeathByBearTag>())
            {
                var dist = math.distancesq(transform.ValueRO.Position, body.ValueRO.Destination);
                if (dist > 0.1f) continue;

                body.ValueRW.IsStopped = true;
                var pos = transform.ValueRO.Position;

                var foundValidPosition = false;
                var fleePos = pos;

                for (var i = 0; i < 30; i++)
                {
                    var randomDir = _random.NextFloat2Direction();
                    var randomDist = _random.NextFloat(5f, 30f);
                    var randomOffset = new float3(randomDir.x * randomDist, 0f, randomDir.y * randomDist);
                    var targetPos = pos + randomOffset;

                    if (NavMesh.SamplePosition(targetPos, out var navHit, 15f, NavMesh.AllAreas))
                    {
                        fleePos = navHit.position;
                        foundValidPosition = true;
                    }
                    
                    if (foundValidPosition) break;
                }
                
                if (!foundValidPosition) continue;
                body.ValueRW.SetDestination(fleePos);
                body.ValueRW.IsStopped = false;

                //check distance to destination
                //get a random point on the navmesh within a distance of the entity
            }
        }
    }
    
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(FleeSystem))]
    public partial struct StartFleeSystem : ISystem
    {
        private Random _random;
        
        public void OnCreate(ref SystemState state)
        {
            _random = Random.CreateFromIndex((uint)System.DateTime.Now.Millisecond - 7u);
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<StartFleeFlag, LocalTransform, AgentBody, DeathByBearTag>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, body, entity) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRW<AgentBody>>()
                         .WithAll<StartFleeFlag, DeathByBearTag>().WithEntityAccess())
            {
                state.EntityManager.SetComponentEnabled<StartFleeFlag>(entity, false);

                body.ValueRW.IsStopped = true;
                var pos = transform.ValueRO.Position;
                var foundValidPosition = false;
                var fleePos = pos;

                for (var i = 0; i < 30; i++)
                {
                    var randomDir = _random.NextFloat2Direction();
                    var randomDist = _random.NextFloat(5f, 30f);
                    var randomOffset = new float3(randomDir.x * randomDist, 0f, randomDir.y * randomDist);
                    var targetPos = pos + randomOffset;

                    if (NavMesh.SamplePosition(targetPos, out var navHit, 15f, NavMesh.AllAreas))
                    {
                        fleePos = navHit.position;
                        foundValidPosition = true;
                    }
                    
                    if (foundValidPosition) break;
                }
                
                if (!foundValidPosition) continue;
                body.ValueRW.SetDestination(fleePos);
                body.ValueRW.IsStopped = false;
            }
        }
    }
}