using Component;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Animations
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class AnimatorMoveSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<AnimatorRefComponent>();
        }

        protected override void OnUpdate()
        {
            foreach (var (trans, animator) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<AnimatorRefComponent>>())
            {
                var goTransform = animator.ValueRO.TransGo;

                goTransform.Value.position = trans.ValueRO.Position;
                goTransform.Value.rotation = trans.ValueRO.Rotation;
            }
        }
    }
}