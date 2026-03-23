using Authoring;
using Component;
using Component.NPCs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Player
{
    [UpdateAfter(typeof(BearAttackSystem))]
    public partial struct ObstacleAttackSystem : ISystem
    {
        private EntityQuery _obstacleQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var elapsedTime = SystemAPI.Time.ElapsedTime;
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            var killList = new NativeList<Entity>(Allocator.Temp);
            var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            
            foreach (var (transform, attack) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<BearAttack>>())
            {
                if (attack.ValueRO.HasHit) continue;
                if (attack.ValueRO.FrameToHit > elapsedTime) continue;
                
                hits.Clear();
                var origin = transform.ValueRO.Position;
                collisionWorld.SphereCastAll(
                    origin + (transform.ValueRO.Up() * 0.5f),
                    attack.ValueRO.Radius,
                    transform.ValueRO.Forward(),
                    attack.ValueRO.DistanceForward,
                    ref hits,
                    CollisionFilter.Default);

                foreach (var hit in hits)
                {
                    var e = hit.Entity;
                    if (!SystemAPI.HasComponent<HitsToKill>(e)) continue;

                    var hitsToKill = SystemAPI.GetComponent<HitsToKill>(e);
                    hitsToKill.Value--;

                    if (hitsToKill.Value <= 0)
                    {
                        if (SystemAPI.HasBuffer<Child>(e))
                        {
                            var linked = SystemAPI.GetBuffer<Child>(e);
                            for (var i = 0; i < linked.Length; i++)
                                killList.Add(linked[i].Value);
                        }
                        else
                            killList.Add(e);
                    }
                    else
                        SystemAPI.SetComponent(e, hitsToKill);
                }
            }

            state.EntityManager.AddComponent<KillTag>(killList.AsArray());
            
            hits.Dispose();
            killList.Dispose();
        }
    }
}
