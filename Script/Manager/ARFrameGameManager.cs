using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;

namespace ViitorCloud.ARModelViewer {

    [RequireComponent(typeof(ARRaycastManager))]
    public class ARFrameGameManager : MonoBehaviour {
        [SerializeField] private ARPlaneManager planeManager;
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        private ARRaycastManager m_RaycastManager;

        [SerializeField] private Color[] colorFrame;

        [SerializeField]
        private GameObject[] selectedTrueImage;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject m_PlacedPrefab;

        private UnityEvent placementUpdate;

        [SerializeField] private GameObject planeDetectionCanvas;
        [SerializeField] private GameObject tapToPlace;
        [SerializeField] private GameObject lowerButton;
        private int colorTempCount = 0;
        private int touchTempCount = 0;
        public bool touchStart;

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
            if (touchTempCount <= 0) {
                tapToPlace.SetActive(true);
            }
        }

        private bool TryGetTouchPosition(out Vector2 touchPosition) {
            if (Input.touchCount > 0) {
                touchTempCount++;
                tapToPlace.SetActive(false);
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        private void Update() {
            if (touchStart) {
                Debug.Log("Raycast Blocked");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon) && Physics.Raycast(ray, out hit, 100.0f, 5)) {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                if (spawnedObject == null) {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, Quaternion.identity);
                    lowerButton.SetActive(true);
                    SpawnObjectData(spawnedObject);
                } else {
                    if (spawnedObject.name == m_PlacedPrefab.name) {
                        //repositioning of the object
                        spawnedObject.transform.position = hitPose.position;
                    } else {
                        Destroy(spawnedObject);

                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, Quaternion.identity);
                        SpawnObjectData(spawnedObject);
                    }
                }
                placementUpdate.Invoke();
            }
        }

        private void SpawnObjectData(GameObject spawnObj) {
            spawnObj.GetComponent<Canvas>().worldCamera = Camera.main;
            spawnObj.GetComponent<ThreeDARFrameCanvas>().DataToDisplay(DataForAllScene.Instance.imageForFrame, colorFrame[colorTempCount]);
            SelectedColorTickOnOff(colorTempCount);
        }

        private void SelectedColorTickOnOff(int indexStatus) {
            for (int i = 0; i <= selectedTrueImage.Length; i++) {
                if (i == indexStatus) {
                    selectedTrueImage[i].SetActive(true);
                } else {
                    selectedTrueImage[i].SetActive(false);
                }
            }
        }

        public void FrameColorChangeOnButton(int index) {
            SelectedColorTickOnOff(colorTempCount);
            spawnedObject.GetComponent<ThreeDARFrameCanvas>().FrameColorChange(colorFrame[index]);
            colorTempCount = index;
        }

        public void OnButtonFrameRotate() {
            spawnedObject.GetComponent<ThreeDARFrameCanvas>().RotateTheImage();
        }

        public void TouchOnOffClicked(bool value) {
            touchStart = value;
        }
    }
}