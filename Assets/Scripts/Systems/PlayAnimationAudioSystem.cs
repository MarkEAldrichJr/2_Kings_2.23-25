using System;
using Authoring;
using Component;
using Component.NPCs;
using Mono;
using Unity.Entities;

namespace Systems
{
    public partial struct PlayAnimationAudioSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            HandleBearSounds(ref state);
            HandleChildSounds(ref state);
            HandleElishaSounds(ref state);
        }

        private void HandleBearSounds(ref SystemState state)
        {
            foreach (var (audioRef, animationState) in SystemAPI
                         .Query<RefRW<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<BearTag>())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;

                var soundClipEnum = animationState.ValueRO.Value switch
                {
                    AnimationStateEnum.Idle => SoundClipEnum.Idle,
                    AnimationStateEnum.Walk => SoundClipEnum.Walk,
                    AnimationStateEnum.Run => SoundClipEnum.Run,
                    AnimationStateEnum.Jump => SoundClipEnum.Jump,
                    AnimationStateEnum.Attack => SoundClipEnum.Attack,
                    AnimationStateEnum.Prone => SoundClipEnum.Idle,
                    AnimationStateEnum.Fear => SoundClipEnum.Idle,
                    _ => throw new ArgumentOutOfRangeException()
                };

                audioRef.ValueRW.AudioControllerGo.Value.SetAudioState(soundClipEnum);
            }
        }

        private void HandleChildSounds(ref SystemState state)
        {
            foreach (var (audioRef, animationState, entity) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<ChildTag>()
                         .WithEntityAccess())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                
                var soundClipType = SoundClipEnum.Run;
                if (SystemAPI.IsComponentEnabled<AttackFlag>(entity))
                    soundClipType = SoundClipEnum.Attack;
                else if (SystemAPI.IsComponentEnabled<FleeFlag>(entity))
                    soundClipType = SoundClipEnum.Fear;
                else if (SystemAPI.IsComponentEnabled<MoveToTargetFlag>(entity))
                    soundClipType = SoundClipEnum.Run;
                else if (SystemAPI.IsComponentEnabled<SneakFlag>(entity))
                    soundClipType = SoundClipEnum.Walk;
                
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(soundClipType);
            }
        }

        private void HandleElishaSounds(ref SystemState state)
        {
            foreach (var (audioRef, animState) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<ElishaFaith>())
            {
                if (!animState.ValueRO.HasChangedThisFrame) continue;
                var soundClip = animState.ValueRO.Value switch
                {
                    AnimationStateEnum.Idle => SoundClipEnum.Idle,
                    AnimationStateEnum.Walk => SoundClipEnum.Walk,
                    AnimationStateEnum.Fear => SoundClipEnum.Fear,
                    _ => throw new ArgumentOutOfRangeException()
                };
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(soundClip);
            }
        }
    }
}