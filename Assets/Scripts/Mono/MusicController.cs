using UnityEngine;

namespace Mono
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;
    
        private void Update()
        {
            if (audioClips.Length == 0) return;
            if (audioSource.isPlaying) return;
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.Play();
        }
    }
}