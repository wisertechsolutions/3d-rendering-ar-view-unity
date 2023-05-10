using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ViitorCloud.ARModelViewer {

    [RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
    public class PlaceObject : MonoBehaviour {
        [SerializeField] private ARCameraManager arCameraManager;
        [SerializeField] private Camera m_Camera;
        [SerializeField] private GameObject m_3dModelNonAR;
        [SerializeField] private GameObject m_3dModelAR;
        [SerializeField] private GameObject m_aRSessionOrigin;
        [SerializeField] private GameObject m_aRDefaultPlane;
        [SerializeField] private ARRaycastManager arRaycastManager;
        [SerializeField] private ARPlaneManager arPlaneManager;
        [SerializeField] private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        public bool isDone {
            get {
                return m_isDone;
            }
            set {
                m_isDone = value;
                CheckIsArModelPlaced();
            }
        }

        private void CheckIsArModelPlaced() {
            if (GameManager.instance.arMode) {
                GameManager.instance.btnTouchOnOff.SetActive(m_isDone);
                GameManager.instance.btnSpawnAR.SetActive(m_isDone);
            } else {
                GameManager.instance.btnTouchOnOff.SetActive(true);
                GameManager.instance.btnSpawnAR.SetActive(false);
            }
        }

        [SerializeField]
        private bool m_isDone;

        private void Awake() {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            ARCameraOnOff();
        }

        private void OnEnable() {
            EnhancedTouch.Touch.onFingerDown += FingerDown;
        }

        private void OnDisable() {
            EnhancedTouch.Touch.onFingerDown -= FingerDown;
        }

        private void FingerDown(EnhancedTouch.Finger finger) {
            if (finger.index != 0 || isDone) {
                return;
            }

            if (arRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)) {
                bool isOverUI = MouseOverUILayerObject.IsPointerOverUIObject();
                Debug.Log($"isOverUI {isOverUI}");
                if (!isOverUI) {
                    foreach (ARRaycastHit hit in hits) {
                        Pose pose = hit.pose;
                        GameManager.instance.aRParent.transform.SetPositionAndRotation(pose.position, pose.rotation);
                        GameManager.instance.aRParent.originalRotation = pose.rotation;
                        isDone = true;
                    }
                }
            }
        }

        public void ARCameraOnOff() {
            GameManager.instance.arMode = !GameManager.instance.arMode;
            arCameraManager.enabled = !arCameraManager.enabled;
            arRaycastManager.enabled = !arRaycastManager.enabled;
            arPlaneManager.enabled = !arPlaneManager.enabled;
            m_Camera.enabled = !m_Camera.enabled;
            m_3dModelNonAR.SetActive(!m_3dModelNonAR.activeInHierarchy);
            m_3dModelAR.SetActive(!m_3dModelAR.activeInHierarchy);
            m_aRSessionOrigin.SetActive(!m_aRSessionOrigin.activeInHierarchy);
            m_aRDefaultPlane.SetActive(!m_aRDefaultPlane.activeInHierarchy);
            CheckIsArModelPlaced();
        }

        public void PlaceARObjectAgain() {
            isDone = false;
            GameManager.instance.aRParent.transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}