using Authoring;
using Authoring.Elisha;
using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct ChildSpawnerSystem : ISystem
    {
        private EntityQuery _elishaQuery;
        private Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DifficultySettings>();
            
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
            //stop spawning if the framerate drops too low.
            var timeLastFrame = SystemAPI.Time.DeltaTime;
            if (timeLastFrame > 0.033333f) return;
            
            var elishaPos = _elishaQuery.GetSingleton<LocalTransform>().Position;
            var difSettings = SystemAPI.GetSingleton<DifficultySettings>();
            
            var prefabs = SystemAPI.GetSingleton<EntityPrefabComponent>();
            var roll = _random.NextFloat(0f, 1f);

            Entity prefab;
            if (roll < difSettings.SpawnBaseChance)
                prefab = prefabs.BaseChild;
            else if (roll < difSettings.SpawnFastChance)
                prefab = prefabs.FastChild;
            else if (roll < difSettings.SpawnTankyChance)
                prefab = prefabs.TankyChild;
            else
                prefab = prefabs.LargeChild;

            var currentFrame = SystemAPI.Time.ElapsedTime;
            
            foreach (var (settings, current) in SystemAPI
                         .Query<RefRO<DifficultySettings>, RefRW<DifficultyCurrent>>())
            {
                #region SpawnNewChild

                if (currentFrame < current.ValueRO.SpawnFrame) continue;
                current.ValueRW.TimeToSpawnNext *= settings.ValueRO.SpawnTimerRateChange;

                current.ValueRW.TimeToSpawnNext = math.clamp(current.ValueRO.TimeToSpawnNext, 0.75f,
                    double.MaxValue);
                
                current.ValueRW.SpawnFrame = currentFrame + current.ValueRO.TimeToSpawnNext;

                var foundValidPos = false;
                var spawnPos = elishaPos;
                for (var i = 0; i < 30; i++)
                {
                    var randomDir = _random.NextFloat2Direction();
                    var randomDist = _random.NextFloat(
                        settings.ValueRO.MinMaxSpawnDistance.x,
                        settings.ValueRO.MinMaxSpawnDistance.y);
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
                #endregion
            }
        }
    }
}