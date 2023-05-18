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
        [SerializeField] private ARRaycastManager arRaycastManager;
        [SerializeField] private ARPlaneManager arPlaneManager;
        [SerializeField] private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        [SerializeField] private bool zoomAnimationDone;
        public bool IsDone {
            get {
                return m_isDone;
            }
            set {
                m_isDone = value;
                CheckIsArModelPlaced();
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
            InvokeRepeating(nameof(CheckForTrackables), 0f, 1f);
        }

        private void OnDisable() {
            EnhancedTouch.Touch.onFingerDown -= FingerDown;
            CancelInvoke(nameof(CheckForTrackables));
        }

        private void CheckIsArModelPlaced() {
            if (GameManager.instance.arMode) {
                GameManager.instance.btnTouchOnOff.SetActive(IsDone);
                GameManager.instance.btnSpawnAR.SetActive(IsDone);
            } else {
                GameManager.instance.btnTouchOnOff.SetActive(true);
                GameManager.instance.btnSpawnAR.SetActive(false);
            }
        }

        private void FingerDown(EnhancedTouch.Finger finger) {
            if (finger.index != 0 || IsDone) {
                return;
            }

            if (arRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)) {
                if (!MouseOverUILayerObject.IsPointerOverUIObject()) {
                    foreach (ARRaycastHit hit in hits) {
                        GameManager.instance.aRParent.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                        GameManager.instance.aRParent.originalRotation = hit.pose.rotation;
                        IsDone = true;
                        if (!zoomAnimationDone) {
                            GameManager.instance.panelZoomInOut.SetActive(true);
                            zoomAnimationDone = true;
                        }
                    }
                }
            }
        }

        void CheckForTrackables() {
            GameManager.instance.panelScanFloor.SetActive(!(arPlaneManager.trackables.count > 0));
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
            CheckIsArModelPlaced();
        }

        public void PlaceARObjectAgain() {
            IsDone = false;
            GameManager.instance.aRParent.transform.position = new Vector3(Constant.aRPosition, Constant.aRPosition, Constant.aRPosition);
        }
    }
}