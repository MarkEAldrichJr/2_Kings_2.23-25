using Unity.Entities;
using UnityEngine;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    [DisallowMultipleComponent]
    public class ThirdPersonPlayerAuthoring : MonoBehaviour
    {
        public GameObject controlledCharacter;
        public GameObject controlledCamera;

        public class Baker : Baker<ThirdPersonPlayerAuthoring>
        {
            public override void Bake(ThirdPersonPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ThirdPersonPlayer
                {
                    ControlledCharacter = GetEntity(authoring.controlledCharacter, TransformUsageFlags.Dynamic),
                    ControlledCamera = GetEntity(authoring.controlledCamera, TransformUsageFlags.Dynamic),
                });
                AddComponent<ThirdPersonPlayerInputs>(entity);
            }
        }
    }
}