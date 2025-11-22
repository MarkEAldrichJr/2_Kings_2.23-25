using System;
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
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioState = idleAudio;
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
            _audioSource.clip = clip;
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.Play();
        }
    }

    public enum SoundClipEnum
    {
        Attack, Run, Walk, Jump, Fear, Idle
    }
}
