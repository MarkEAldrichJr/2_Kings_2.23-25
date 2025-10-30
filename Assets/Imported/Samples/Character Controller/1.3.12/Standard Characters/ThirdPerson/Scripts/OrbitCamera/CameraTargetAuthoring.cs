using Unity.Entities;
using UnityEngine;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts.OrbitCamera
{
    [DisallowMultipleComponent]
    public class CameraTargetAuthoring : MonoBehaviour
    {
        public GameObject target;

        public class Baker : Baker<CameraTargetAuthoring>
        {
            public override void Bake(CameraTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraTarget
                {
                    TargetEntity = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}