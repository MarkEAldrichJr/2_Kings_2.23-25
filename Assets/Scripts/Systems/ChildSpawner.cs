using Authoring;
using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;

namespace Systems
{
    public partial struct ChildSpawner : ISystem
    {
        private EntityQuery _elishaQuery;
        private Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _random = Random.CreateFromIndex((uint)(SystemAPI.Time.DeltaTime * 100000));
            state.RequireForUpdate<EntityPrefabComponent>();
            
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<DifficultySettings, DifficultyCurrent>();
            state.RequireForUpdate(state.GetEntityQuery(builder));

            var elishaBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FollowTrail, LocalTransform>()
                .WithNone<RequirePlayerTag>();
            _elishaQuery = state.GetEntityQuery(elishaBuilder);
            state.RequireForUpdate(_elishaQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var elishaPos = _elishaQuery.GetSingleton<LocalTransform>().Position;
            var prefab = SystemAPI.GetSingleton<EntityPrefabComponent>().BaseChild;
            var currentFrame = SystemAPI.Time.ElapsedTime;
            
            foreach (var (settings, current) in SystemAPI
                         .Query<RefRO<DifficultySettings>, RefRW<DifficultyCurrent>>())
            {
                #region SpawnNewChild
                if (current.ValueRO.SpawnFrame < currentFrame)
                {
                    current.ValueRW.SpawnFrame = CalculateNextSpawnFrame(currentFrame,
                        current.ValueRO.DifficultyIncreaseCount, settings.ValueRO.SpawnTimerMax,
                        settings.ValueRO.SpawnTimerRateChange);

                    var foundValidPos = false;
                    var spawnPos = elishaPos;
                    for (var i = 0; i < 30; i++)
                    {
                        var randomDir = _random.NextFloat2Direction();
                        var randomDist = _random.NextFloat(20f, 300f);
                        var randomOffset = new float3(randomDir.x * randomDist, 0f,
                            randomDir.y * randomDist);
                        var targetPos = elishaPos + randomOffset;

                        if (NavMesh.SamplePosition(targetPos, out var navHit,
                                15f, NavMesh.AllAreas))
                        {
                            spawnPos = navHit.position;
                            foundValidPos = true;
                        }

                        if (foundValidPos) break;
                    }

                    if (foundValidPos)
                    {
                        var spawn = state.EntityManager.Instantiate(prefab);
                        var trans = state.EntityManager.GetComponentData<LocalTransform>(spawn);
                
                        trans.Position = spawnPos;
                        state.EntityManager.SetComponentData(spawn, trans);
                    }
                }
                #endregion

                #region RaiseDifficulty
                if (current.ValueRO.DifficultyIncreaseFrame < currentFrame)
                {
                    current.ValueRW.DifficultyIncreaseFrame =
                        currentFrame + settings.ValueRO.DifficultySpikeRate;

                    current.ValueRW.DifficultyIncreaseCount++;
                }
                #endregion
            }
        }

        [BurstCompile]
        public static double CalculateNextSpawnFrame(
            double currentFrame,            //The frame this method is called on
            uint numberDifficultySpikes,    //How many times difficulty has been increased
            double maxTimer,                //initial timer maximum
            double changeRate)              //percent decrease in time after each difficulty increase
        {
            return currentFrame + maxTimer - maxTimer * (changeRate * numberDifficultySpikes);
        }
    }
}