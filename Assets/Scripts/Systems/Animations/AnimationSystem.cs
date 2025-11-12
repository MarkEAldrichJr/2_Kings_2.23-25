using Component;
using Mono;
using Unity.Entities;

namespace Systems
{
    public partial class AnimationSystem : SystemBase
    {
        private EntityManager _entityManager;
        
        protected override void OnCreate()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            SetAnimationStateOnGOs();
        }

        private void SetAnimationStateOnGOs()
        {
            foreach (var (animationState, e) in SystemAPI
                         .Query<RefRW<AnimationStateComp>>()
                         .WithEntityAccess())
            {
                if (!animationState.ValueRO.HasChangedThisFrame) continue;
                
                var animator = _entityManager.GetComponentObject<AnimatorController>(e);
                animator.ChangeAnimation(animationState.ValueRO.Value);

                animationState.ValueRW.HasChangedThisFrame = false;
            }
        }
    }
}