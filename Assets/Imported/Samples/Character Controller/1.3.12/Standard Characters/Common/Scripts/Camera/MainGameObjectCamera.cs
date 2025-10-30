using UnityEngine;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts.Camera
{
    public class MainGameObjectCamera : MonoBehaviour
    {
        public static UnityEngine.Camera Instance;

        private void Awake()
        {
            Instance = GetComponent<UnityEngine.Camera>();
        }
    }
}