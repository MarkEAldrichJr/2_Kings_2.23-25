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
        
        public class DifficultyBaker : Baker<DifficultyAuthoring>
        {
            public override void Bake(DifficultyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                
                AddComponent(entity, new DifficultySettings
                {
                    SpawnTimerRateChange = authoring.spawnTimeDropPerSpawn,
                    MinMaxSpawnDistance = new float2(authoring.minSpawnDistance, authoring.minSpawnDistance)
                });
                
                AddComponent(entity, new DifficultyCurrent
                {
                    SpawnFrame = authoring.initialSpawnTime,
                    TimeToSpawnNext = authoring.initialSpawnTime
                });
            }
        }
    }

    public struct DifficultySettings : IComponentData
    {
        public float SpawnTimerRateChange;
        public float2 MinMaxSpawnDistance;
    }

    public struct DifficultyCurrent : IComponentData
    {
        public double SpawnFrame;
        public double TimeToSpawnNext;
    }
}
