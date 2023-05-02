using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ViitorCloud.ARModelViewer {
    public class Rotate3dGameObject : MonoBehaviour {
        private InputSystemManager touchControls;

        private void Awake() {
            touchControls = new InputSystemManager();
        }

        private void OnEnable() {
            touchControls.Enable();
            EnhancedTouch.TouchSimulation.Enable();

            //EnhancedTouch.Touch.onFingerDown += FingerDown;
        }

        private void OnDisable() {
            touchControls.Disable();
            EnhancedTouch.TouchSimulation.Disable();

            //EnhancedTouch.Touch.onFingerDown -= FingerDown;
        }

        //private void FingerDown(EnhancedTouch.Finger finger) {

        //}

        private void Start() {
            touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
            touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
        }

        private void Update() {
            foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                    //Debug.Log("TouchPhase.Began");
                } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                    transform.Rotate(0f, touch.delta.x * Constant.rotateSpeed, 0f);
                    //Debug.Log("TouchPhase.Moved");
                } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) {
                    Debug.Log("TouchPhase.Ended");
                } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary) {
                    //Debug.Log("TouchPhase.Stationary");
                }
            }
        }

        private void StartTouch(InputAction.CallbackContext context) {
            Debug.Log("Touch started " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());

        }

        private void EndTouch(InputAction.CallbackContext context) {
            Debug.Log("Touch ended");
        }

        public void ResetRotation() {
            transform.rotation = Quaternion.identity;
        }
    }
}