using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace Mono
{
    public class RestartGame : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private float timer;

        private float _timeLeft;
        private string _timeWritten;
        private string _timeLastFrame;
        private bool _isReadyForInput;

        private void Awake()
        {
            _timeLeft = timer;
            _isReadyForInput = false;
            timerText.text = timer.ToString(CultureInfo.InvariantCulture);

            promptText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (CheckReadyForInput())
                ActOnInput();
        }

        private bool CheckReadyForInput()
        {
            if (!_isReadyForInput)
            {
                _timeLeft -= Time.deltaTime;
                _timeWritten = math.round(_timeLeft).ToString(CultureInfo.InvariantCulture);
                
                if (_timeWritten != _timeLastFrame)
                {
                    timerText.text = _timeWritten;
                    _timeLastFrame = _timeWritten;
                }

                if (_timeLeft > 0)
                {
                    _isReadyForInput = false;
                    return false;
                }

                _isReadyForInput = true;
                promptText.gameObject.SetActive(true);
                timerText.gameObject.SetActive(false);
            }
            return true;
        }

        private static void ActOnInput()
        {
            var gamepad = Gamepad.current;
            if (gamepad is { wasUpdatedThisFrame: true })
            {
                foreach (var control in gamepad.allControls)
                {
                    if (control is ButtonControl { wasPressedThisFrame: true })
                    {
                        LoadFirstScene();
                        return;
                    }
                }
            }
            if (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame ||
                 Keyboard.current.enterKey.wasPressedThisFrame))
            {
                LoadFirstScene();
            }
        }
        
        private static void LoadFirstScene() => SceneManager.LoadScene(0);
    }
}