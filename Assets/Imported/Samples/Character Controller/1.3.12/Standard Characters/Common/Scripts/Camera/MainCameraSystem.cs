using Unity.Entities;
using Unity.Transforms;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts.Camera
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class MainCameraSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (MainGameObjectCamera.Instance != null && SystemAPI.HasSingleton<MainEntityCamera>())
            {
                //TODO: Make MainEntityCamera and MainGameObjectCamera non-singletons, and get them references to each other.
                //Maybe have the cameras keep a reference to the OrbitCamera entity and have them track their Transforms?
                //TODO: Make Camera and bear child of a prefab that handles inputs.  Makes life easier I think, instead of setting world position of camera?
                var mainEntityCameraEntity = SystemAPI.GetSingletonEntity<MainEntityCamera>();
                var targetLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(mainEntityCameraEntity);
                MainGameObjectCamera.Instance.transform.SetPositionAndRotation(targetLocalToWorld.Position, targetLocalToWorld.Rotation);
            }
        }
    }
}