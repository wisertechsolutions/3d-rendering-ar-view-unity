using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


public class Rotate3dGameObject : MonoBehaviour {
    private NewInput touchControls;

    private void Awake() {
        touchControls = new NewInput();
    }

    private void OnEnable() {
        touchControls.Enable();
        EnhancedTouch.TouchSimulation.Enable();

        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable() {
        touchControls.Disable();
        EnhancedTouch.TouchSimulation.Disable();

        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger) {

    }

    private void Start() {
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void Update() {
        //Debug.Log(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches);
        foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches) {
            //Debug.Log(touch.phase == UnityEngine.InputSystem.TouchPhase.Began);
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                Debug.Log("TouchPhase.Began");
            } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                Debug.Log("TouchPhase.Moved");
            } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) {
                Debug.Log("TouchPhase.Ended");
            }
        }
    }

    private void StartTouch(InputAction.CallbackContext context) {
        Debug.Log("Touch started " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());

    }

    private void EndTouch(InputAction.CallbackContext context) {
        Debug.Log("Touch ended");
    }
}