using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FaithAuthoring : MonoBehaviour
    {
        [SerializeField] private float maxFaith = 100f;
        [SerializeField] private float damagePerChild = 5f;
        [SerializeField] private float faithRegen = 0.3f;
        
        private class FaithAuthoringBaker : Baker<FaithAuthoring>
        {
            public override void Bake(FaithAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new ElishaFaith
                {
                    CurrentFaith = authoring.maxFaith,
                    DamagePerChild = authoring.damagePerChild,
                    FaithMax = authoring.maxFaith,
                    FaithRegen = authoring.faithRegen,
                    NumChildren = 0u
                });
            }
        }
    }
}