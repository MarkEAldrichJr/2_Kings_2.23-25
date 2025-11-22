using Mono;
using Unity.Entities;
using UnityEngine;

public class PlayAudioClipOnSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioClip runAudioClip;
    [SerializeField] private AudioClip attackAudioClip;
    
    public class PlayAudioClipOnSpawnBaker : Baker<PlayAudioClipOnSpawnAuthoring>
    {
        public override void Bake(PlayAudioClipOnSpawnAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new PlayAudioClipData
            {
                AudioClip = authoring.audioClip
            });
            SetComponentEnabled<PlayAudioClipData>(e, false);
        }
    }
}

public struct PlayAudioClipData : IComponentData, IEnableableComponent
{
    public UnityObjectRef<AudioClip> AudioClip;
    
}

public partial struct PlayAudioClipOnSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (audioClipData, playAudioClip) in SystemAPI
                     .Query<RefRO<PlayAudioClipData>, EnabledRefRW<PlayAudioClipData>>())
        {
            AudioController.Instance.PlayAudioClip(audioClipData.ValueRO.AudioClip);
            playAudioClip.ValueRW = false;
        }
    }
}

