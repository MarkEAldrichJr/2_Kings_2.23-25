using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mono
{
    public class RestartGame : MonoBehaviour
    {
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable() => restartButton.onClick.AddListener(LoadFirstScene);
        private void OnDisable() => restartButton.onClick.RemoveListener(LoadFirstScene);
        
        private static void LoadFirstScene()
        {
            SceneManager.LoadScene(0);
        }
    }
}