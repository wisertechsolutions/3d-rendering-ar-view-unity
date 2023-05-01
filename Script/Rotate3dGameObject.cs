using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class Rotate3dGameObject : MonoBehaviour {
    private NewInput touchControls;
    public float rotatespeed = 10f;
    private float _startingPosition;

    private void Awake() {
        touchControls = new NewInput();
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
        //foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {          
        //    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {
        //        _startingPosition = touch.screenPosition.x;
        //        Debug.Log("TouchPhase.Began");
        //    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
        //        //transform.Rotate(0f, touch.delta.x * 0.5f, 0f);
        //        if (_startingPosition > touch.screenPosition.x) {
        //            transform.Rotate(Vector3.up, -rotatespeed * Time.deltaTime);
        //        } else if (_startingPosition < touch.screenPosition.x) {
        //            transform.Rotate(Vector3.up, rotatespeed * Time.deltaTime);
        //        }
        //        Debug.Log("TouchPhase.Moved");
        //    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) {
        //        Debug.Log("TouchPhase.Ended");
        //    } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary) {
        //        _startingPosition = touch.screenPosition.x;
        //        Debug.Log("TouchPhase.Stationary");
        //    }
        //}

        foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {              
                //Debug.Log("TouchPhase.Began");
            } else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                transform.Rotate(0f, touch.delta.x * 0.5f, 0f);               
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
        transform.rotation= Quaternion.identity;
    }
}