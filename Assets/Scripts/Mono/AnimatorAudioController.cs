using System;
using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mono
{
    /// <summary>
    /// This is a temporary sound system that works on all animators.  In the future, I'd like to build an entity-based system that
    /// uses predefined audio tracks that only run when within a set distance from the player.
    /// </summary>
    public class AnimatorAudioController : MonoBehaviour
    {
        [SerializeField] private float timerMax = 5f;
        [SerializeField] private AudioClip idleAudio;
        [SerializeField] private AudioClip runAudio;
        [SerializeField] private AudioClip walkAudio;
        [SerializeField] private AudioClip jumpAudio;
        [SerializeField] private AudioClip fearAudio;
        [SerializeField] private AudioClip attackAudio;
        
        private AudioClip _audioState;
        private AudioSource _audioSource;
        private float _timer;

        private EntityQuery _bearEntityQuery;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioState = idleAudio;
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _bearEntityQuery = entityManager.CreateEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BearTag, LocalTransform>());
        }

        public void SetAudioState(SoundClipEnum state)
        {
            _audioState = state switch
            {
                SoundClipEnum.Attack => attackAudio,
                SoundClipEnum.Run => runAudio,
                SoundClipEnum.Jump => jumpAudio,
                SoundClipEnum.Fear => fearAudio,
                SoundClipEnum.Idle => idleAudio,
                SoundClipEnum.Walk => walkAudio,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
            PlayAudio(_audioState);
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < timerMax) return;
            _timer = 0f;
            
            PlayAudio(_audioState);
        }

        private void PlayAudio(AudioClip clip)
        {
            var dist = GetShortDistance.GetShortestDistance(ref _bearEntityQuery, transform.position);
            if (dist > 50f) return;
            dist = math.clamp(
                math.remap( 50f, 0f, .1f, 1f,dist),
                0.01f,
                1f);
            
            _audioSource.volume = 0.7f * dist;
            
            _audioSource.clip = clip;
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.Play();
        }
    }

    [BurstCompile]
    public static class GetShortDistance
    {
        [BurstCompile]
        public static float GetShortestDistance(ref EntityQuery query, in float3 position)
        {
            var bears =  query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var shortestDistance = float.MaxValue;
            foreach (var bear in bears)
            {
                var dist = math.distance(bear.Position, position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                }
            }

            bears.Dispose();
            return shortestDistance;
        }
    }
    
    
    
    public enum SoundClipEnum
    {
        Attack, Run, Walk, Jump, Fear, Idle
    }
}
