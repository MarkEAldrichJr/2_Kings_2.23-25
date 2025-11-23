using Authoring;
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
            //stop spawning if the framerate drops too low.
            var timeLastFrame = SystemAPI.Time.DeltaTime;
            if (timeLastFrame > 0.033333f) return;
            
            var elishaPos = _elishaQuery.GetSingleton<LocalTransform>().Position;
            var prefab = SystemAPI.GetSingleton<EntityPrefabComponent>().BaseChild;
            var currentFrame = SystemAPI.Time.ElapsedTime;
            
            foreach (var (settings, current) in SystemAPI
                         .Query<RefRO<DifficultySettings>, RefRW<DifficultyCurrent>>())
            {
                #region SpawnNewChild

                if (currentFrame < current.ValueRO.SpawnFrame) continue;
                current.ValueRW.TimeToSpawnNext *= settings.ValueRO.SpawnTimerRateChange;
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