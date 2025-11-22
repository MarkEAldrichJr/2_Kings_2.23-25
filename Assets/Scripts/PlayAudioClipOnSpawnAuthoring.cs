using Mono;
using Unity.Entities;
using UnityEngine;

public class PlayAudioClipOnSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    
    public class PlayAudioClipOnSpawnBaker : Baker<PlayAudioClipOnSpawnAuthoring>
    {
        public override void Bake(PlayAudioClipOnSpawnAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new PlayAudioClipOnSpawnData
            {
                AudioClip = authoring.audioClip
            });
        }
    }
}

public struct PlayAudioClipOnSpawnData : IComponentData, IEnableableComponent
{
    public UnityObjectRef<AudioClip> AudioClip;
}

public partial struct PlayAudioClipOnSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (audioClipData, playAudioClip) in SystemAPI
                     .Query<PlayAudioClipOnSpawnData, EnabledRefRW<PlayAudioClipOnSpawnData>>())
        {
            AudioController.Instance.PlayAudioClip(audioClipData.AudioClip);
            playAudioClip.ValueRW = false;
        }
    }
}