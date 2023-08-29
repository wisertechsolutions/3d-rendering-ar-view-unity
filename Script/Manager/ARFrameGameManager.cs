using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
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

        [Header("TestMode")]
        public bool testMode;

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
#if UNITY_EDITOR || UNITY_WIN
            if (Input.GetKeyDown(KeyCode.Z) && testMode) {
                TestModeFunc();
            }
#endif

            if (!MouseOverUILayerObject.IsPointerOverUIObject()) {
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
        }

        private void TestModeFunc() {
            planeDetectionCanvas.SetActive(false);

            spawnedObject = Instantiate(m_PlacedPrefab, Vector3.zero, Quaternion.identity);
            lowerButton.SetActive(true);
            SpawnObjectData(spawnedObject);
        }

        private void SpawnObjectData(GameObject spawnObj) {
            spawnObj.GetComponent<Canvas>().worldCamera = Camera.main;
            spawnObj.GetComponent<ThreeDARFrameCanvas>().DataToDisplay(DataForAllScene.Instance.imageForFrame, colorFrame[colorTempCount]);
            SelectedColorTickOnOff(colorTempCount);
        }

        private void SelectedColorTickOnOff(int indexStatus) {
            for (int i = 0; i <= selectedTrueImage.Length - 1; i++) {
                if (i == indexStatus) {
                    selectedTrueImage[i].SetActive(true);
                } else {
                    selectedTrueImage[i].SetActive(false);
                }
            }
        }

        public void FrameColorChangeOnButton(int index) {
            spawnedObject.GetComponent<ThreeDARFrameCanvas>().FrameColorChange(colorFrame[index]);
            colorTempCount = index;
            SelectedColorTickOnOff(colorTempCount);
        }

        public void OnButtonFrameRotate() {
            spawnedObject.GetComponent<ThreeDARFrameCanvas>().RotateTheImage();
        }

        public void OnBackButtonPress() {
            CallPreviousSceneOfNative();
        }

        private void CallPreviousSceneOfNative() {
#if UNITY_ANDROID
            // Get the current activity
            IntPtr jclass = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
            IntPtr jactivity = AndroidJNI.GetStaticObjectField(jclass, AndroidJNI.GetStaticFieldID(jclass, "currentActivity", "Landroid/app/Activity;"));

            // Get the finish() method and call it on the current activity
            IntPtr jmethod = AndroidJNI.GetMethodID(jclass, "finish", "()V");
            AndroidJNI.CallVoidMethod(jactivity, jmethod, new jvalue[] { });

            // Release references to avoid memory leaks
            AndroidJNI.DeleteLocalRef(jactivity);
            AndroidJNI.DeleteLocalRef(jclass);
#elif UNITY_IOS
            _closeUnityAndReturnToiOS("Description of iOS on Back Button Clicked", "iOS Back Button Clicked")
#endif
        }

#if UNITY_IOS
    [DllImport("__Internal")]
	extern static private void _closeUnityAndReturnToiOS(string description, string msg);
#endif
    }
}