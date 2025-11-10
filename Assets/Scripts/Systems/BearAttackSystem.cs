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
            state.RequireForUpdate<DeathByBearTag>();
            _bearQuery = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, BearAttack>());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (control, attack) in SystemAPI
                         .Query<RefRO<ThirdPersonCharacterControl>, RefRW<BearAttack>>())
            {
                if (!control.ValueRO.Attack) continue;

                if (attack.ValueRO.FrameCooldownFinishes > (uint)SystemAPI.Time.ElapsedTime)
                    continue; 
                
                attack.ValueRW.FrameCooldownFinishes =
                    (uint)SystemAPI.Time.ElapsedTime + attack.ValueRO.CooldownTime;

                attack.ValueRW.FrameStopDamage =
                    (uint)SystemAPI.Time.ElapsedTime + attack.ValueRO.StopDamageTime;
            }

            var attacks = _bearQuery.ToComponentDataArray<BearAttack>(Allocator.TempJob);
            var transforms = _bearQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var killList = new NativeList<Entity>(Allocator.TempJob);
            var killListWriter = killList.AsParallelWriter();

            var numAttacks = 0;
            foreach (var attack in attacks)
            {
                if (attack.FrameStopDamage < (uint)SystemAPI.Time.ElapsedTime)
                {
                    numAttacks++;
                }
            }

            if (numAttacks == 0)
            {
                attacks.Dispose();
                transforms.Dispose();
                killList.Dispose();
                
                return;
            }
            
            var scheduleParallel = new KillEvilChildrenJob
            {
                TimeElapsed = (uint)SystemAPI.Time.ElapsedTime,
                BearAttacks = attacks,
                Transforms = transforms,
                KillList = killListWriter
            }.ScheduleParallel(state.Dependency);
            scheduleParallel.Complete();
            
            attacks.Dispose();
            transforms.Dispose();
            
            state.EntityManager.DestroyEntity(killList.AsArray());
            
            killList.Dispose();
        }
    }

    [BurstCompile]
    public partial struct KillEvilChildrenJob : IJobEntity
    {
        [ReadOnly] public uint TimeElapsed;
        [ReadOnly] public NativeArray<BearAttack> BearAttacks;
        [ReadOnly] public NativeArray<LocalTransform> Transforms;
        [WriteOnly] public NativeList<Entity>.ParallelWriter KillList;
        
        private void Execute(Entity entity, in LocalTransform transform, in DeathByBearTag tag)
        {
            var length = Transforms.Length;

            for (var i = 0; i < length; i++)
            {
                if (BearAttacks[i].FrameStopDamage > TimeElapsed)
                {
                    var attackPosition = Transforms[i].Position +
                                         Transforms[i].Forward() * BearAttacks[i].DistanceForward;

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