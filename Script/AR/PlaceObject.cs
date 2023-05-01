using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour {
    //[SerializeField] private GameObject prefab;
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_3dModel;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isDone;

    private void Awake() {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable() {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();

        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable() {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();

        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger) {
        if (finger.index != 0 || isDone) {
            return;
        }

        if (arRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)) {
            foreach (ARRaycastHit hit in hits) {
                Pose pose = hit.pose;
                //GameObject obj = Instantiate(prefab, pose.position, pose.rotation);
                GameManager.instance.uIManager.ModelParent.SetPositionAndRotation(pose.position, pose.rotation);
                isDone = true;
            }
        }
    }

    public void ARCameraOnOff() {
        arCameraManager.enabled = !arCameraManager.enabled;
        arRaycastManager.enabled = !arRaycastManager.enabled;
        arPlaneManager.enabled = !arPlaneManager.enabled;
        m_Camera.enabled = !m_Camera.enabled;
        m_3dModel.SetActive(!m_3dModel.activeInHierarchy);
    }
}