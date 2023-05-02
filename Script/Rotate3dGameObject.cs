using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;

namespace ViitorCloud.ARModelViewer {
    public class Rotate3dGameObject : MonoBehaviour {
        private InputSystemManager touchControls;
        private float initialFingersDistance;
        private Vector3 initialScale;

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
            if (EnhancedTouch.Touch.activeFingers.Count == 1) {
                foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                        //Debug.Log("TouchPhase.Began");
                    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                        transform.Rotate(0f, -touch.delta.x * Constant.rotateSpeed, 0f);
                        //Debug.Log("TouchPhase.Moved");
                    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) {
                        //Debug.Log("TouchPhase.Ended");
                    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary) {
                        //Debug.Log("TouchPhase.Stationary");
                    }
                }
            }

            if (EnhancedTouch.Touch.activeFingers.Count == 2) {
                if (transform.localScale.x > 0.1f && transform.localScale.x < 1f) {
                    //foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {                        
                    //    EnhancedTouch.Touch t1 = EnhancedTouch.Touch.activeTouches[0];
                    //    EnhancedTouch.Touch t2 = EnhancedTouch.Touch.activeTouches[1];

                    foreach (var (t1, t2) in from EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches let t1 = EnhancedTouch.Touch.activeTouches[0] let t2 = EnhancedTouch.Touch.activeTouches[1] select (t1, t2)) {

                        if (t1.phase == UnityEngine.InputSystem.TouchPhase.Began || t2.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                            initialFingersDistance = Vector2.Distance(t1.screenPosition, t2.screenPosition);
                            initialScale = transform.localScale;
                        } else if (t1.phase == UnityEngine.InputSystem.TouchPhase.Moved || t2.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                            var currentFingersDistance = Vector2.Distance(t1.screenPosition, t2.screenPosition);
                            var scaleFactor = currentFingersDistance / initialFingersDistance;

                            float scale = initialScale.x * scaleFactor;
                            if (scale > 0.11f && scale < 0.99f) {
                                transform.localScale = new Vector3(scale, scale, scale);
                            }
                        }
                    }
                }
            }
        }    

        private void StartTouch(InputAction.CallbackContext context) {
            //Debug.Log("Touch started " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        }

        private void EndTouch(InputAction.CallbackContext context) {
            //Debug.Log("Touch ended");
        }

        public void ResetRotation() {
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}