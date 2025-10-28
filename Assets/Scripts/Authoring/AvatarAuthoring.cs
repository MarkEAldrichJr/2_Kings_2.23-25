using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class AvatarAuthoring : MonoBehaviour
    {
        public GameObject avatar;
        private class AvatarAuthoringBaker : Baker<AvatarAuthoring>
        {
            public override void Bake(AvatarAuthoring authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponentObject(e, new AnimatorModelData
                {
                    Value = authoring.avatar
                });
            }
        }
    }
    
    public class AnimatorModelData : IComponentData
    {
        public GameObject Value;
    }

    public class AnimatorModelReference : IComponentData
    {
        public AnimatorController Controller;
    }
}