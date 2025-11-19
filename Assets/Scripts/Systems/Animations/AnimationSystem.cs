using Component;
using Mono;
using Unity.Entities;

namespace Systems.Animations
{
    public partial class AnimationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (animationState, animRef) in SystemAPI
                         .Query<RefRW<AnimationStateComp>, RefRO<AnimatorRefComponent>>())
            { 
                if (!animationState.ValueRO.HasChangedThisFrame) continue;

                var animator = animRef.ValueRO.AnimatorGo.Value.GetComponent<AnimatorController>();
                animator.ChangeAnimation(animationState.ValueRO.Value);

                animationState.ValueRW.HasChangedThisFrame = false;
            }
        }
    }
}