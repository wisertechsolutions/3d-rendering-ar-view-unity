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
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

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

        private bool isReadyToMove;

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
                //GameManager.instance.btnTouchOnOff.SetActive(IsDone);
                GameManager.instance.btnSpawnAR.SetActive(IsDone);
                GameManager.instance.btnToggles.SetActive(IsDone);
            } else {
                GameManager.instance.btnTouchOnOff.SetActive(true);
                GameManager.instance.btnSpawnAR.SetActive(false);
            }
        }

        private void Update() {
            if (isReadyToMove && Input.touchCount > 1) {
                isReadyToMove = false;
                return;
            } else if (!isReadyToMove && Input.touchCount == 0) {
                isReadyToMove = true;
            }

            if (isReadyToMove && !MouseOverUILayerObject.IsPointerOverUIObject() && IsDone && Input.touchCount == 1) {
                Vector2 touchPosition = Input.GetTouch(0).position;

                if (arRaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon)) {
                    var hitPose = s_Hits[0].pose;
                    GameManager.instance.objParent.transform.SetPositionAndRotation(hitPose.position, GameManager.instance.objParent.transform.rotation);

                    // GameManager.instance.objParent.originalRotation = hitPose.rotation;
                }
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
                        //GameManager.instance.objParent.originalRotation = hit.pose.rotation;
                        GameManager.instance.objParent.originalRotation = Quaternion.identity;
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

        private void CheckForTrackables() {
            GameManager.instance.panelScanFloor.SetActive(!(arPlaneManager.trackables.count > 0));
            if (arPlaneManager.trackables.count > 0) {
                GameManager.instance.panelTapToPlaceObject.SetActive(true);
                CancelInvoke(nameof(CheckForTrackables));
            }
        }

        public void PlaceARObjectAgain() {
            IsDone = false;
            GameManager.instance.objParent.transform.position = new Vector3(Constant.aRPosition, Constant.aRPosition, Constant.aRPosition);
            GameManager.instance.objParent.ResetRotation();
        }
    }
}