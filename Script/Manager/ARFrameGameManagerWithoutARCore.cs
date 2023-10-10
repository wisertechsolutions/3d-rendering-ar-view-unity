using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ViitorCloud.ARModelViewer {

    [RequireComponent(typeof(ARRaycastManager))]
    public class ARFrameGameManagerWithoutARCore : MonoBehaviour {
        [SerializeField] private ARPlaneManager planeManager;
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        private ARRaycastManager arRaycastManager;

        [SerializeField] private Color[] colorFrame;

        [SerializeField]
        private GameObject[] selectedTrueImage;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject m_PlacedPrefab;

        private UnityEvent placementUpdate;

        [SerializeField] private Transform mainCam;
        [SerializeField] private GameObject planeDetectionCanvas;
        [SerializeField] private GameObject waitingPanel;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private GameObject tapToPlace;
        [SerializeField] private GameObject lowerButton;
        private int colorTempCount = 0;
        private int touchTempCount = 0;

        [Header("TestMode")]
        public bool testMode;

        private float fixedZPos = 15f;

        private float rotationValueOnZ = 5f;
        private bool waitingLoaderIsOn;

        private float distanceToMaintain = 1.5f; //5Feet
        private float distance;
        private float minDistance = 0f;
        private Transform planeSuccessTransform;

        [Header("Distance")]
        [SerializeField] private bool raycastLogic;

        private float maRayDistance = 100f;

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
            arRaycastManager = GetComponent<ARRaycastManager>();

            if (placementUpdate == null)
                placementUpdate = new UnityEvent();
        }

        private void OnEnable() {
            // InvokeRepeating(nameof(ARPlaneDistanceTrackingUpdate), 0f, 1f);
            if (!raycastLogic) {
                Invoke(nameof(ARPlaneDistanceTrackingUpdate), 1f);
            }
        }

        private void OnDisable() {
            CancelInvoke(nameof(ARPlaneDistanceTrackingUpdate));
        }

        private void Start() {
            waitingLoaderIsOn = true;
            StartCoroutine(WaitingPanelHandler());
            spawnedObject = null;
            spawned = false;
            if (mainCam == null) {
                mainCam = Camera.main.transform;
            }
            if (!Input.gyro.enabled) {
                Input.gyro.enabled = true;
            }
            planeDetectionCanvas.SetActive(true);
            tapToPlace.SetActive(false);
        }

        private IEnumerator WaitingPanelHandler() {
            yield return new WaitForSeconds(2);
            waitingPanel.SetActive(false);
            waitingLoaderIsOn = false;
        }

        private void Update() {
            if (waitingLoaderIsOn)
                return;

#if UNITY_EDITOR || UNITY_WIN
            if (Input.GetKeyDown(KeyCode.Z) && testMode) {
                TestModeFunc();
            }
#endif

            if (raycastLogic) {
                // RayCast Logic for Distance Calculations
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maRayDistance)) {
                    Debug.Log("Hit object name: " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name.Contains("ARPlane")) {
                        string targetObjectName = hit.collider.gameObject.name;
                        //Debug.Log("Hit object name: " + targetObjectName);
                        distance = Vector3.Distance(Camera.main.transform.position, hit.transform.position);
                        Debug.Log($" RayCast Hit is - {hit.collider.gameObject}");
                        Debug.Log($" Distance with RayCast is - {distance}");
                        DistanceChecker();
                    }
                } else {
                    Debug.Log("else distance");
                    errorPanel.SetActive(false);
                }
            }

            Vector3 acceleration = Input.acceleration;
            // Check if phone is held straight
            float tiltThresholdX = 0.2f; // Adjust this value as per your requirement

            float tiltThresholdY = 0.8f; // Adjust this value as per your requirement
            if (!spawned && !waitingLoaderIsOn) {
                if (Mathf.Abs(acceleration.x) < tiltThresholdX && Mathf.Abs(acceleration.y) > tiltThresholdY) {
                    Debug.Log("Phone is held properly straight.");
                    planeDetectionCanvas.SetActive(false);

                    tapToPlace.SetActive(true);

                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                        if (Input.touchCount > 0 && touchTempCount <= 0) {
                            touchTempCount++;
                            tapToPlace.SetActive(false);
                        }
                        var hitPosVector = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        // Raycast hits are sorted by distance, so the first one
                        // will be the closest hit.
                        var newHitPosition = new Vector3(hitPosVector.x, hitPosVector.y, fixedZPos);
                        if (spawnedObject == null) {
                            spawnedObject = Instantiate(m_PlacedPrefab, newHitPosition, Quaternion.identity, mainCam);
                            spawnedObject.transform.localPosition = new Vector3(0, 0, fixedZPos);
                            spawnedObject.transform.localEulerAngles = Vector3.zero;

                            lowerButton.SetActive(true);
                            SpawnObjectData(spawnedObject);
                            spawned = true;
                        }
                    }
                    placementUpdate.Invoke();
                } else {
                    Debug.Log("Phone is not held straight.");
                    planeDetectionCanvas.SetActive(true);
                    tapToPlace.SetActive(false);
                }
            } else if (spawned) {
                if (Input.GetMouseButton(0)) {
                    Swipe();
                    Debug.Log("Swipe Started");
                }
            }
        }

        #region ARPlaneDistanceTracking

        private void ARPlaneDistanceTrackingUpdate() {
            foreach (ARPlane arPlane in planeManager.trackables) {
                if (arPlane.trackingState == TrackingState.Tracking) {
                    distance = Vector3.Distance(Camera.main.transform.position, arPlane.transform.position);
                    Debug.Log("Distance: " + distance);
                    DistanceChecker();
                }
            }
            Invoke(nameof(ARPlaneDistanceTrackingUpdate), 0.5f);
        }

        private void DistanceChecker() {
            if (distance >= distanceToMaintain) {
                errorPanel.SetActive(false);

                //planeSuccessTransform = arPlane.transform;

                // planeManager.enabled = false; //Divya Plane Distance Optimization
                // Invoke(nameof(ARPlaneDistanceTrackingAfterOneUpdate), 0.5f); //Divya Plane Distance Optimization
                // CancelInvoke(nameof(ARPlaneDistanceTrackingUpdate)); //Divya Plane Distance Optimization
            } else {
                errorPanel.SetActive(true);
            }
        }

        //Divya Plane Distance Optimization
        /*private void ARPlaneDistanceTrackingAfterOneUpdate() {
            Debug.Log("ARPlaneDistanceTrackingAfterOneUpdate - " + planeSuccessTransform.position);
            distance = Vector3.Distance(Camera.main.transform.position, planeSuccessTransform.position);
            Debug.Log("Distance: " + distance);
            if (distance >= distanceToMaintain) {
                isDistanceMaintain = true;
            } else {
                isDistanceMaintain = false;
            }
            Debug.Log("Distance Bool: " + isDistanceMaintain);
            Invoke(nameof(ARPlaneDistanceTrackingAfterOneUpdate), 0.1f);
        }*/

        private float CheckMinDistance(float distanceForCheck) {
            if (distanceForCheck <= minDistance) {
                minDistance = distanceForCheck;
            }
            Debug.Log("Min Distance: " + minDistance);
            return minDistance;
        }

        #endregion ARPlaneDistanceTracking

        private void TestModeFunc() {
            planeDetectionCanvas.SetActive(false);
            tapToPlace.SetActive(false);
            errorPanel.SetActive(false);

            spawnedObject = Instantiate(m_PlacedPrefab, Vector3.zero, Quaternion.identity);
            spawned = true;
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

        public void OnBackButtonReset() {
            if (spawned) {
                spawnedObject.transform.localPosition = new Vector3(0, 0, fixedZPos);
                spawnedObject.transform.localEulerAngles = Vector3.zero;
            }
        }

        public void RotateOnZOnClickButtons(bool isRToL) {
            if (isRToL) {
                spawnedObject.transform.Rotate(Vector3.forward, rotationValueOnZ);
            } else {
                spawnedObject.transform.Rotate(Vector3.forward, -rotationValueOnZ);
            }
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
            Application.Unload();
#endif
        }

        #region SwipeLogic

        private Vector2 _firstPressPos;
        private Vector2 _secondPressPos;
        private Vector2 _currentSwipe;

        [Header("Swipe Logic")]
        [SerializeField] private float swipeValue = 10f;

        private bool spawned;

        public void Swipe() {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawned) {
                _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && spawned) {
                _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

                _currentSwipe.Normalize();

                if (LeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(-swipeValue, 0, 0) * Time.deltaTime;
                } else if (RightSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(swipeValue, 0, 0) * Time.deltaTime;
                } else if (UpLeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(-swipeValue, swipeValue, 0) * Time.deltaTime;
                } else if (UpRightSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(swipeValue, swipeValue, 0) * Time.deltaTime;
                } else if (DownLeftSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(-swipeValue, -swipeValue, 0) * Time.deltaTime;
                } else if (DownRightSwipe(_currentSwipe)) {
                    spawnedObject.transform.localPosition += new Vector3(swipeValue, -swipeValue, 0) * Time.deltaTime;
                }
            }
            spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, spawnedObject.transform.localPosition.y, fixedZPos);
            Debug.Log("Swipe Ended");
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