using Unity.Entities;
using UnityEngine;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts.Camera
{
    [DisallowMultipleComponent]
    public class MainEntityCameraAuthoring : MonoBehaviour
    {
        public class Baker : Baker<MainEntityCameraAuthoring>
        {
            public override void Bake(MainEntityCameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MainEntityCamera>(entity);
            }
        }
    }
}
