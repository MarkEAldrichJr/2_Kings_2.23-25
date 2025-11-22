using UnityEngine;

namespace Mono
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;
        
        [SerializeField] private int poolSize = 50;

        private AudioSource[] _audioSourcePool;
        private int _poolIndex;
        
        private void Awake()
        {
            if (Instance) Destroy(Instance);
            Instance = this;
        }

        private void Start()
        {
            _audioSourcePool = new AudioSource[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                var newAudioSourceObject = new GameObject($"Audio Source-{i}");
                newAudioSourceObject.transform.SetParent(transform);
                var newAudioSource = newAudioSourceObject.AddComponent<AudioSource>();
                _audioSourcePool[i] = newAudioSource;
            }
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            var currentAudioSource = _audioSourcePool[_poolIndex];
            currentAudioSource.clip = audioClip;
            currentAudioSource.Play();
            _poolIndex++;
            _poolIndex %= _audioSourcePool.Length;
        }
    }
}
