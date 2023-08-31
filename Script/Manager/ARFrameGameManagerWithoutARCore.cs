using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace ViitorCloud.ARModelViewer {

    [RequireComponent(typeof(ARRaycastManager))]
    public class ARFrameGameManagerWithoutARCore : MonoBehaviour {
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

        private float fixedZPos = 5f;

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
            planeDetectionCanvas.SetActive(false);
            tapToPlace.SetActive(true);
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
            if (MouseOverUILayerObject.IsPointerOverUIObject())
                return;

            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null) {
                if (Input.touchCount > 0 && touchTempCount <= 0) {
                    touchTempCount++;
                    tapToPlace.SetActive(false);
                }
                var hitPosVector = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var newHitPosition = new Vector3(hitPosVector.x, hitPosVector.y, fixedZPos);
                Debug.Log("New hit Position = " + newHitPosition);
                if (spawnedObject == null) {
                    spawnedObject = Instantiate(m_PlacedPrefab, newHitPosition, Quaternion.identity, Camera.main.transform);
                    lowerButton.SetActive(true);
                    SpawnObjectData(spawnedObject);
                }
                placementUpdate.Invoke();
            } else {
                Swipe();
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
            SendMessage("iOSBridge", "CloseUnityAndReturnToiOS");
#endif
        }

        //_closeUnityAndReturnToiOS("Description of iOS on Back Button Clicked", "iOS Back Button Clicked");

#if UNITY_IOS
    [DllImport("__Internal")]
	extern static private void _closeUnityAndReturnToiOS(string description, string msg);
#endif

        #region SwipeLogic

        private Vector2 _firstPressPos;
        private Vector2 _secondPressPos;
        private Vector2 _currentSwipe;
        private float swipeValue = 10f;

        public void Swipe() {
            if (Input.GetMouseButtonDown(0) && spawnedObject != null) {
                _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            if (Input.GetMouseButtonUp(0) && spawnedObject != null) {
                _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

                _currentSwipe.Normalize();

                if (LeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(-swipeValue, 0, 0) * Time.deltaTime;
                } else if (RightSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(swipeValue, 0, 0) * Time.deltaTime;
                } else if (UpLeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(-swipeValue, swipeValue, 0) * Time.deltaTime;
                } else if (UpRightSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(swipeValue, swipeValue, 0) * Time.deltaTime;
                } else if (DownLeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(-swipeValue, -swipeValue, 0) * Time.deltaTime;
                } else if (DownRightSwipe(_currentSwipe)) {
                    spawnedObject.transform.position += new Vector3(swipeValue, -swipeValue, 0) * Time.deltaTime;
                }
            }
        }

        private bool LeftSwipe(Vector2 Swipe) {
            return _currentSwipe.x < 0 && _currentSwipe.y < 0.5f && _currentSwipe.y > -0.5f;
        }

        private bool RightSwipe(Vector2 Swipe) {
            return _currentSwipe.x > 0 && _currentSwipe.y < 0.5f && _currentSwipe.y > -0.5f;
        }

        private bool UpLeftSwipe(Vector2 Swipe) {
            return _currentSwipe.y > 0 && _currentSwipe.x < 0f;
        }

        private bool UpRightSwipe(Vector2 Swipe) {
            return _currentSwipe.y > 0 && _currentSwipe.x > 0f;
        }

        private bool DownLeftSwipe(Vector2 Swipe) {
            return _currentSwipe.y < 0 && _currentSwipe.x < 0f;
        }

        private bool DownRightSwipe(Vector2 Swipe) {
            return _currentSwipe.y < 0 && _currentSwipe.x > 0f;
        }

        #endregion SwipeLogic
    }
}