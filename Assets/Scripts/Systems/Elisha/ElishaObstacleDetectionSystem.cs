using Authoring;
using Authoring.Elisha;
using Component;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Elisha
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FollowTrailSystem))]
    public partial struct ElishaObstacleDetectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (agent, trans, faith) in SystemAPI
                         .Query<RefRW<AgentBody>, LocalTransform, ElishaFaith>()
                         .WithAll<FollowTrail>()
                         .WithNone<FollowTrailStartTag, RequirePlayerTag>())
            {
                var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                collisionWorld.SphereCastAll(trans.Position, 0.6f,
                    trans.Forward(), 1f,
                    ref hits, CollisionFilter.Default);

                var blocked = false;
                foreach (var hit in hits)
                {
                    if (SystemAPI.HasComponent<ObstacleTag>(hit.Entity))
                    {
                        blocked = true;
                        break;
                    }
                }
                hits.Dispose();

                var underAttack = faith.TimeSinceLastDamage < 1.1f;

                agent.ValueRW.IsStopped = blocked || underAttack;
            }
        }
    }
}