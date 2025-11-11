using Component;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateAfter(typeof(AnimationStateDiscoverySystem))]
    public partial struct BearAttackSystem : ISystem
    {
        private EntityQuery _bearQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _bearQuery = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, BearAttack>());
            
            state.RequireForUpdate<DeathByBearTag>();
            state.RequireForUpdate(_bearQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var elapsedTime = SystemAPI.Time.ElapsedTime;
            
            foreach (var (control, attack) in SystemAPI
                         .Query<RefRO<ThirdPersonCharacterControl>, RefRW<BearAttack>>())
            {
                if (!control.ValueRO.Attack) continue;
                if (attack.ValueRO.FrameCooldownFinishes > elapsedTime)
                    continue; 
                
                attack.ValueRW.FrameCooldownFinishes =
                    elapsedTime + attack.ValueRO.CooldownTime;
                attack.ValueRW.FrameStopDamage =
                    elapsedTime + attack.ValueRO.StopDamageTime;
                attack.ValueRW.FrameToStart = 
                    elapsedTime + attack.ValueRO.StartTime;
            }

            var attacks = _bearQuery.ToComponentDataArray<BearAttack>(Allocator.TempJob);
            var transforms = _bearQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var killList = new NativeList<Entity>(Allocator.TempJob);
            var killListWriter = killList.AsParallelWriter();
            
            var scheduleParallel = new KillEvilChildrenJob
            {
                TimeElapsed = elapsedTime,
                BearAttacks = attacks,
                Transforms = transforms,
                KillList = killListWriter
            }.ScheduleParallel(state.Dependency);
            
            scheduleParallel.Complete();
            state.EntityManager.DestroyEntity(killList.AsArray());
            
            attacks.Dispose();
            transforms.Dispose();
            killList.Dispose();
        }
    }

    [BurstCompile]
    public partial struct KillEvilChildrenJob : IJobEntity
    {
        [ReadOnly] public double TimeElapsed;
        [ReadOnly] public NativeArray<BearAttack> BearAttacks;
        [ReadOnly] public NativeArray<LocalTransform> Transforms;
        [WriteOnly] public NativeList<Entity>.ParallelWriter KillList;
        
        private void Execute(Entity entity, in LocalTransform transform, in DeathByBearTag tag)
        {
            var length = Transforms.Length;

            for (var i = 0; i < length; i++)
            {
                if (TimeElapsed > BearAttacks[i].FrameToStart)
                {
                    if (TimeElapsed < BearAttacks[i].FrameStopDamage)
                    {
                        var attackPosition = Transforms[i].Position +
                                             (Transforms[i].Forward() *
                                             BearAttacks[i].DistanceForward);

                        var distanceToAttack = math.distance(attackPosition, transform.Position);
                        if (distanceToAttack < BearAttacks[i].Radius)
                        {
                            KillList.AddNoResize(entity);
                        }
                    }
                }
            }
        }
    }
}