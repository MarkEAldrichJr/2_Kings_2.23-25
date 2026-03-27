using Authoring;
using Authoring.Child;
using Component;
using Component.NPCs;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Player
{
    [UpdateAfter(typeof(Animations.AnimationStateDiscoverySystem))]
    public partial struct BearAttackSystem : ISystem
    {
        private EntityQuery _bearQuery;
        private ComponentLookup<Knockback> _largeChildLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _bearQuery = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, BearAttack>());
            
            state.RequireForUpdate<ChildTag>();
            state.RequireForUpdate(_bearQuery);
            _largeChildLookup = state.GetComponentLookup<Knockback>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _largeChildLookup.Update(ref state);
            var elapsedTime = SystemAPI.Time.ElapsedTime;
            
            foreach (var (control, attack) in SystemAPI
                         .Query<RefRO<ThirdPersonCharacterControl>, RefRW<BearAttack>>())
            {
                if (!control.ValueRO.Attack) continue;
                if (attack.ValueRO.FrameCooldownFinishes > elapsedTime)
                    continue; 
                
                attack.ValueRW.FrameCooldownFinishes =
                    elapsedTime + attack.ValueRO.CooldownTime;
                
                attack.ValueRW.FrameToHit = 
                    elapsedTime + attack.ValueRO.AnimationDelay;
                
                attack.ValueRW.HasHit = false;
            }

            var attacks = _bearQuery.ToComponentDataArray<BearAttack>(Allocator.TempJob);
            var transforms = _bearQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var killList = new NativeList<Entity>(Allocator.TempJob);
            var killListWriter = killList.AsParallelWriter();
            var knockbackRequests = new NativeList<KnockbackRequestData>(Allocator.TempJob);
            var knockbackWriter = knockbackRequests.AsParallelWriter();
            
            var scheduleParallel = new KillEvilChildrenJob
            {
                TimeElapsed = elapsedTime,
                BearAttacks = attacks,
                Transforms = transforms,
                KillList = killListWriter,
                BearEntities = _bearQuery.ToEntityArray(Allocator.TempJob),
                KnockbackRequests = knockbackWriter,
                LargeChildLookup = _largeChildLookup
            }.ScheduleParallel(state.Dependency);
            
            scheduleParallel.Complete();
            
            foreach (var request in knockbackRequests)
            {
                if (state.EntityManager.HasComponent<KnockbackRequest>(request.Target))
                    // Accumulate — just overwrite, one knockback per frame is fine
                    state.EntityManager.SetComponentData(request.Target, new KnockbackRequest
                    {
                        Direction = request.Direction,
                        Force = request.Force
                    });
                else
                    state.EntityManager.AddComponentData(request.Target, new KnockbackRequest
                    {
                        Direction = request.Direction,
                        Force = request.Force
                    });
            }
            state.EntityManager.AddComponent<KillTag>(killList.AsArray());
            
            attacks.Dispose();
            transforms.Dispose();
            killList.Dispose();
        }
    }

    public struct KnockbackRequestData
    {
        public Entity Target;
        public float3 Direction;
        public float Force;
    }
    
    /// <summary>
    /// Takes all the children, gets their distance from the BearAttackEntity,
    /// and adds them to the kill list if they're close enough.
    /// </summary>
    [BurstCompile]
    public partial struct KillEvilChildrenJob : IJobEntity
    {
        [ReadOnly] public double TimeElapsed;
        [ReadOnly] public NativeArray<BearAttack> BearAttacks;
        [ReadOnly] public NativeArray<LocalTransform> Transforms;
        [WriteOnly] public NativeList<Entity>.ParallelWriter KillList;
        
        // Add to KillEvilChildrenJob fields:
        [ReadOnly] public NativeArray<Entity> BearEntities;
        [WriteOnly] public NativeList<KnockbackRequestData>.ParallelWriter KnockbackRequests;
        [ReadOnly] public ComponentLookup<Knockback> LargeChildLookup;
        
        [BurstCompile]
        private void Execute(Entity entity, in LocalTransform transform, ref HitsToKill hits)
        {
            for (var i = 0; i < Transforms.Length; i++)
            {
                if (BearAttacks[i].HasHit) continue;
                if (BearAttacks[i].FrameToHit > TimeElapsed) continue;
                
                //Get position of the attack sphere
                var attackPosition = Transforms[i].Position +
                                     Transforms[i].Forward() *
                                     BearAttacks[i].DistanceForward;

                var distanceToAttack = math.distance(attackPosition, transform.Position);
                if (distanceToAttack < BearAttacks[i].Radius)
                {
                    //Large children add a knockback force when attacked.
                    if (LargeChildLookup.TryGetComponent(entity, out var knockbackComp))
                    {
                        // Get horizontal-only direction from child to bear (away from child)
                        var towardBear = Transforms[i].Position - transform.Position;
                        towardBear.y = 0f;
                        var horizontalDir = math.normalizesafe(towardBear, math.forward());

                        // Build a fixed-angle launch vector (tune the angle to taste)
                        var launchAngle = math.radians(knockbackComp.LaunchAngle); // e.g. 30 degrees
                        var knockDir = new float3(
                            horizontalDir.x * math.cos(launchAngle),
                            math.sin(launchAngle),
                            horizontalDir.z * math.cos(launchAngle)
                        );
                        // knockDir is now a unit vector at a consistent upward angle

                        KnockbackRequests.AddNoResize(new KnockbackRequestData
                        {
                            Target = BearEntities[i],
                            Direction = knockDir,
                            Force = knockbackComp.Force
                        });
                    }
                    
                    hits.Value--;
                    if (hits.Value <= 0)
                    {
                        KillList.AddNoResize(entity);
                    }
                }
            }
        }
    }
    
    
    [UpdateAfter(typeof(ObstacleAttackSystem))]
    public partial struct BearAttackResetSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var elapsedTime = SystemAPI.Time.ElapsedTime;
            foreach (var attack in SystemAPI
                         .Query<RefRW<BearAttack>>())
            {
                if (!attack.ValueRO.HasHit && attack.ValueRO.FrameToHit <= elapsedTime)
                {
                    attack.ValueRW.HasHit = true;
                }
            }
        }
    }
}