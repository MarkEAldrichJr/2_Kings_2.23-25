using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class DifficultyAuthoring : MonoBehaviour
    {
        [SerializeField] private float initialSpawnTime;
        [SerializeField] private float spawnTimeDropPerSpawn = 0.95f;
        [SerializeField] private float minSpawnDistance;
        [SerializeField] private float maxSpawnDistance;
        
        [Header("Enemy Spawn Chance")]
        [Range(0f, 1f)]
        [SerializeField] private float spawnBaseChance = 0.55f;
        [Range(0f, 1f)]
        [SerializeField] private float spawnFastChance = 0.75f;
        [Range(0f, 1f)]
        [SerializeField] private float spawnTankyChance = 0.95f;
        
        public class DifficultyBaker : Baker<DifficultyAuthoring>
        {
            public override void Bake(DifficultyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                
                AddComponent(entity, new DifficultySettings
                {
                    SpawnTimerRateChange = authoring.spawnTimeDropPerSpawn,
                    MinMaxSpawnDistance = new float2(authoring.minSpawnDistance, authoring.minSpawnDistance),
                    SpawnBaseChance = authoring.spawnBaseChance,
                    SpawnFastChance = authoring.spawnFastChance,
                    SpawnTankyChance = authoring.spawnTankyChance
                });
                
                AddComponent(entity, new DifficultyCurrent
                {
                    SpawnFrame = authoring.initialSpawnTime,
                    TimeToSpawnNext = authoring.initialSpawnTime,
                });
            }
        }
    }

    public struct DifficultySettings : IComponentData
    {
        public float SpawnTimerRateChange;
        public float2 MinMaxSpawnDistance;
        public float SpawnBaseChance;
        public float SpawnFastChance;
        public float SpawnTankyChance;
    }

    public struct DifficultyCurrent : IComponentData
    {
        public double SpawnFrame;
        public double TimeToSpawnNext;
    }
}
