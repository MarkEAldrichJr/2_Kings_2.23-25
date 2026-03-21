using Authoring;
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
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _bearQuery = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, BearAttack>());
            
            state.RequireForUpdate<ChildTag>();
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
            state.EntityManager.AddComponent<KillTag>(killList.AsArray());
            
            attacks.Dispose();
            transforms.Dispose();
            killList.Dispose();
        }
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
        
        [BurstCompile]
        private void Execute(Entity entity, in LocalTransform transform, ref HitsToKill hits)
        {
            for (var i = 0; i < Transforms.Length; i++)
            {
                if (BearAttacks[i].FrameToStart > TimeElapsed) continue;
                if (TimeElapsed > BearAttacks[i].FrameStopDamage) continue;
                
                //Get position of the attack sphere
                var attackPosition = Transforms[i].Position +
                                     Transforms[i].Forward() *
                                     BearAttacks[i].DistanceForward;

                var distanceToAttack = math.distance(attackPosition, transform.Position);
                if (distanceToAttack < BearAttacks[i].Radius)
                {
                    hits.Value--;
                    if (hits.Value <= 0)
                    {
                        KillList.AddNoResize(entity);
                    }
                }
            }
        }
    }
}