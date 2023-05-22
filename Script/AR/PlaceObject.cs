using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ViitorCloud.ARModelViewer {
    [RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
    public class PlaceObject : MonoBehaviour {
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
            CheckIsArModelPlaced();
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
                        GameManager.instance.objParent.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                        GameManager.instance.objParent.originalRotation = hit.pose.rotation;
                        IsDone = true;
                        GameManager.instance.panelTapToPlaceObject.SetActive(false);
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
            if (arPlaneManager.trackables.count > 0) {
                GameManager.instance.panelTapToPlaceObject.SetActive(true);
                CancelInvoke(nameof(CheckForTrackables));
            }
        }

        public void PlaceARObjectAgain() {
            IsDone = false;
            GameManager.instance.objParent.transform.position = new Vector3(Constant.aRPosition, Constant.aRPosition, Constant.aRPosition);
        }
    }
}