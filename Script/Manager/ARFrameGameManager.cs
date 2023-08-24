using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using System;

namespace ViitorCloud.ARModelViewer {

    [RequireComponent(typeof(ARRaycastManager))]
    public class ARFrameGameManager : MonoBehaviour {
        [SerializeField] private ARPlaneManager planeManager;
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        private ARRaycastManager m_RaycastManager;

        [SerializeField] private Color[] colorFrame;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject m_PlacedPrefab;

        private UnityEvent placementUpdate;

        [SerializeField] private GameObject planeDetectionCanvas;
        [SerializeField] private GameObject tapToPlace;
        [SerializeField] private GameObject lowerButton;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab {
            get {
                return m_PlacedPrefab;
            }
            set {
                m_PlacedPrefab = value;
            }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject {
            get; private set;
        }

        private void Awake() {
            m_RaycastManager = GetComponent<ARRaycastManager>();

            if (placementUpdate == null)
                placementUpdate = new UnityEvent();
        }

        private void Start() {
            planeDetectionCanvas.SetActive(true);
            planeManager.planesChanged += DisableUi;
        }

        private void DisableUi(ARPlanesChangedEventArgs args) {
            planeDetectionCanvas.SetActive(false);
            tapToPlace.SetActive(true);
        }

        private bool TryGetTouchPosition(out Vector2 touchPosition) {
            if (Input.touchCount > 0) {
                tapToPlace.SetActive(false);
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        private void Update() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon) && Physics.Raycast(ray, out hit, 100.0f, 5)) {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                if (spawnedObject == null) {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    lowerButton.SetActive(true);
                    SpawnObjectData(spawnedObject);
                } else {
                    if (spawnedObject.name == m_PlacedPrefab.name) {
                        //repositioning of the object
                        spawnedObject.transform.position = hitPose.position;
                    } else {
                        Destroy(spawnedObject);

                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                        SpawnObjectData(spawnedObject);
                    }
                }
                placementUpdate.Invoke();
            }
        }

        private void SpawnObjectData(GameObject spawnObj) {
            spawnObj.GetComponent<Canvas>().worldCamera = Camera.main;
            spawnObj.GetComponent<ThreeDARFrameCanvas>().DataToDisplay(DataForAllScene.Instance.imageForFrame, colorFrame[0]);
        }

        public void FrameColorChangeOnButton(int index) {
            spawnedObject.GetComponent<ThreeDARFrameCanvas>().FrameColorChange(colorFrame[index]);
        }

        public void OnButtonFrameRotate() {
            spawnedObject.transform.rotation = Quaternion.Euler(0, 0, spawnedObject.transform.rotation.z + 90);
        }
    }
}