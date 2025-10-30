using System;
using Unity.Entities;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts.OrbitCamera
{
    [Serializable]
    public struct CameraTarget : IComponentData
    {
        public Entity TargetEntity;
    }
}
