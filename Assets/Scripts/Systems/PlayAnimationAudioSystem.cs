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
                    _ => throw new ArgumentOutOfRangeException()
                };

                audioRef.ValueRW.AudioControllerGo.Value.SetAudioState(soundClipEnum);
            }
        }

        private void HandleChildSounds(ref SystemState state)
        {
            //Attack
            foreach (var (audioRef, animationState) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<DeathByBearTag, AttackFlag>())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(SoundClipEnum.Attack);
            }
            //Flee
            foreach (var (audioRef, animationState) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<DeathByBearTag, FleeFlag>())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(SoundClipEnum.Fear);
            }
            //Run
            foreach (var (audioRef, animationState) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<DeathByBearTag, MoveToTargetFlag>())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(SoundClipEnum.Run);
            }
            //Sneak
            foreach (var (audioRef, animationState) in SystemAPI
                         .Query<RefRO<AudioRefComponent>, RefRO<AnimationStateComp>>()
                         .WithAll<DeathByBearTag, SneakFlag>())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                audioRef.ValueRO.AudioControllerGo.Value.SetAudioState(SoundClipEnum.Walk);
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