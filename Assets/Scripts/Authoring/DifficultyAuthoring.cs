using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class DifficultyAuthoring : MonoBehaviour
    {
        [SerializeField] private float initialSpawnTime;
        [SerializeField] private float spawnTimeDropPerSpike;
        [SerializeField] private float minSpawnDistance;
        [SerializeField] private float maxSpawnDistance;
        
        [SerializeField] private float initialRunSpeed;
        [SerializeField] private float runSpeedRisePerSpike;
        [SerializeField] private float initialDamageRate;
        [SerializeField] private float damageRateRisePerSpike;
        [SerializeField] private float difficultySpikeRate;
        
        public class DifficultyBaker : Baker<DifficultyAuthoring>
        {
            public override void Bake(DifficultyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                
                AddComponent(entity, new DifficultySettings
                {
                    SpawnTimerMax = authoring.initialSpawnTime,
                    SpawnTimerRateChange = authoring.spawnTimeDropPerSpike,
                    RunSpeedMin = authoring.initialRunSpeed,
                    RunSpeedRateChange = authoring.runSpeedRisePerSpike,
                    DamageRateMin = authoring.initialDamageRate,
                    DamageRateRateChange = authoring.damageRateRisePerSpike,
                    DifficultySpikeRate = authoring.difficultySpikeRate,
                    minMaxSpawnDistance = new float2(authoring.minSpawnDistance, authoring.minSpawnDistance)
                });
                
                AddComponent(entity, new DifficultyCurrent
                {
                    DifficultyIncreaseCount = 0,
                    SpawnFrame = 10,
                    DifficultyIncreaseFrame = 1000,
                    DamageRate = 1
                });
            }
        }
    }

    public struct DifficultySettings : IComponentData
    {
        public float SpawnTimerMax;
        public float SpawnTimerRateChange;
        public float RunSpeedMin;
        public float RunSpeedRateChange;
        public float DamageRateMin;
        public float DamageRateRateChange;
        public float DifficultySpikeRate;
        public float2 minMaxSpawnDistance;
    }

    public struct DifficultyCurrent : IComponentData
    {
        public double SpawnFrame;
        public double DifficultyIncreaseFrame;
        public uint DifficultyIncreaseCount;
        public float DamageRate;
        
    }
}
