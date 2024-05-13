using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ViitorCloud.ARModelViewer {


    public class ARFramePlacementManager : MonoBehaviour {
        [SerializeField] private ARPlaneManager arPlaneManager;
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        [SerializeField] private ARRaycastManager arRaycastManager;

        [SerializeField] private Color[] colorFrame;

        [SerializeField]
        private GameObject[] selectedTrueImage;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject m_PlacedPrefab;

        //private UnityEvent placementUpdate;

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

        private bool waitingLoaderIsOn;
        private float distance;
        private float minDistance = 0f;
        private Vector3 resetPosition;
        private Vector3 resetRotation;
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

            //if (placementUpdate == null)
            //    placementUpdate = new UnityEvent();
        }

        private void Start() {
            waitingLoaderIsOn = true;
            arPlaneManager.planesChanged += OnPlanesChanged;

            StartCoroutine(WaitingPanelHandler());
            spawnedObject = null;

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

        void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs) {
            if (spawnedObject == null) {
                if (arPlaneManager.trackables.count > 0) {
                    //Debug.Log("AR Plane Detected");
                    planeDetectionCanvas.SetActive(false);
                    tapToPlace.SetActive(true);
                } else {
                    planeDetectionCanvas.SetActive(true);
                    tapToPlace.SetActive(false);
                }
            }

            //foreach (var plane in eventArgs.updated) {
            //    // Check if the plane is moving by comparing position and rotation
            //    if (Vector3.Distance(plane.transform.position, plane.transform.position) > 0.01f ||
            //        Quaternion.Angle(plane.transform.rotation, plane.transform.rotation) > 1f) {
            //        Debug.Log("AR Plane is moving");
            //    }
            //}
        }


        private void Update() {
            //#if UNITY_EDITOR || UNITY_WIN
            //            if (Input.GetKeyDown(KeyCode.Z) && testMode) {
            //                TestModeFunc();
            //            }
            //#endif

            TryPlaceObject();
        }

        private void TryPlaceObject() {
            if (waitingLoaderIsOn)
                return;

            if (planeDetectionCanvas.activeSelf) {
                return;
            }
            if (!MouseOverUILayerObject.IsPointerOverUIObject()) {
                if (Input.touchCount > 0) {
                    //Debug.Log("Touch detected");
                    if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null) {
                        Vector2 touchPosition = Input.GetTouch(0).position;
                        if (arRaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon)) {
                            Debug.Log("Ray Hit");
                            Pose hitPose = s_Hits[0].pose;


                            SpawnFrame(hitPose);
                            lowerButton.SetActive(true);
                            tapToPlace.SetActive(false);
                        }
                    } else {
                        //repositioning of the object
                        //spawnedObject.transform.position = hitPose.position;
                        Swipe();
                    }
                    //placementUpdate.Invoke();

                }
            }
        }

        //private bool CheckDeviceHeldStraight() {
        //    Vector3 acceleration = Input.acceleration;
        //    // Check if phone is held straight
        //    float tiltThresholdX = 0.2f; // Adjust this value as per your requirement

        //    float tiltThresholdY = 0.8f; // Adjust this value as per your requirement

        //    if (Mathf.Abs(acceleration.x) < tiltThresholdX && Mathf.Abs(acceleration.y) > tiltThresholdY) {
        //        planeDetectionCanvas.SetActive(false);
        //        tapToPlace.SetActive(true);
        //        return true;
        //    } else {
        //        Debug.Log("Phone is not held straight.");
        //        planeDetectionCanvas.SetActive(true);
        //        tapToPlace.SetActive(false);
        //        return false;
        //    }
        //}

        private void SpawnFrame(Pose hitPose) {
            spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
            //spawnedObject.transform.forward = -hitPose.up;
            resetPosition = spawnedObject.transform.position;
            resetRotation = spawnedObject.transform.eulerAngles;
            SetFrameData();
        }
        private void SetFrameData() {
            spawnedObject.GetComponent<ARFrame3D>().DataToDisplay(DataForAllScene.Instance.TextureForFrame, colorFrame[colorTempCount]);
            SelectedColorTickOnOff(colorTempCount);
        }



        //private bool TryGetTouchPosition(out Vector2 touchPosition) {
        //    if (Input.touchCount > 0) {
        //        touchTempCount++;
        //        tapToPlace.SetActive(false);
        //        touchPosition = Input.GetTouch(0).position;
        //        return true;
        //    }

        //    touchPosition = default;
        //    return false;
        //}


        //private void TestModeFunc() {
        //    planeDetectionCanvas.SetActive(false);

        //    spawnedObject = Instantiate(m_PlacedPrefab, Vector3.zero, Quaternion.identity);
        //    lowerButton.SetActive(true);
        //    SpawnObjectData(spawnedObject);
        //}

        //private void SpawnObjectData(GameObject spawnObj) {
        //    Debug.Log("spawn");
        //    spawnObj.GetComponent<Canvas>().worldCamera = Camera.main;
        //    spawnObj.GetComponent<ARFrame3D>().DataToDisplay(DataForAllScene.Instance.TextureForFrame, colorFrame[colorTempCount]);
        //    SelectedColorTickOnOff(colorTempCount);
        //}

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
            //spawnedObject.GetComponent<ARFrame3D>().FrameColorChange(colorFrame[index]);
            //colorTempCount = index;
            //SelectedColorTickOnOff(colorTempCount);
        }

        public void OnButtonFrameRotate() {
            //spawnedObject.GetComponent<ARFrame3D>().RotateTheImage();
        }

        public void OnBackButtonPress() {
            CallPreviousSceneOfNative();
        }

        public void OnBackButtonReset() {
            if (spawnedObject != null) {
                //spawnedObject.transform.localPosition = new Vector3(0, 0, Constant.fixedZPos);
                spawnedObject.transform.position = resetPosition;
                //spawnedObject.GetComponent<ARFrame3D>().ResetRotation();
            }
        }

        //public void RotateOnZOnClickButtons(bool isRToL) {
        //    if (isRToL) {
        //        spawnedObject.transform.Rotate(Vector3.forward, Constant.rotationValueOnZ);
        //    } else {
        //        spawnedObject.transform.Rotate(Vector3.forward, -Constant.rotationValueOnZ);
        //    }
        //}
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

        //private Vector2 _firstPressPos;
        //private Vector2 _secondPressPos;
        //private Vector2 _currentSwipe;

        //[Header("Swipe Logic")]
        //[SerializeField] private float swipeValue = 10f;

        float moveSpeed = 5f;

        private Vector2 touchStartPos;
        private Vector2 touchEndPos;
        private Vector2 swipeDelta;

        public void Swipe() {

            if (Input.touchCount > 0 && spawnedObject != null) {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began) {
                    touchStartPos = touch.position;
                } else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended) {
                    touchEndPos = touch.position;
                    swipeDelta = touchEndPos - touchStartPos;

                    if (swipeDelta.magnitude > 50) // Minimum swipe distance to consider
                    {
                        // Get the object's local right and forward vectors
                        Vector3 localRight = spawnedObject.transform.right;
                        Vector3 localUp = spawnedObject.transform.up;

                        // Calculate movement direction based on swipe in local space
                        Vector3 moveDirection = localRight * swipeDelta.x + localUp * swipeDelta.y;

                        // Translate the object in its local XY plane
                        spawnedObject.transform.Translate(moveDirection.normalized * moveSpeed * 0.1f * Time.deltaTime, Space.World);
                    }
                }
            }
        }


        //public void Swipe() {
        //    if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject!=null) {
        //        _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //    }
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null) {
        //        _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //        _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

        //        _currentSwipe.Normalize();

        //        if (LeftSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(-swipeValue, 0, 0) * Time.deltaTime;
        //            spawnedObject.transform.localPosition += new Vector3(-swipeValue, swipeValue, 0) * Time.deltaTime;
        //        } else if (RightSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(swipeValue, 0, 0) * Time.deltaTime;
        //        } else if (UpLeftSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(-swipeValue, 0, 0) * Time.deltaTime;
        //            spawnedObject.transform.up += new Vector3(0, swipeValue, 0) * Time.deltaTime;
        //            //spawnedObject.transform.localPosition += new Vector3(-swipeValue, swipeValue, 0) * Time.deltaTime;
        //        } else if (UpRightSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(swipeValue, 0, 0) * Time.deltaTime;
        //            spawnedObject.transform.up += new Vector3(0, swipeValue, 0) * Time.deltaTime;
        //            //spawnedObject.transform.localPosition += new Vector3(swipeValue, swipeValue, 0) * Time.deltaTime;
        //        } else if (DownLeftSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(-swipeValue, 0, 0) * Time.deltaTime;
        //            spawnedObject.transform.up += new Vector3(0, -swipeValue, 0) * Time.deltaTime;
        //            //spawnedObject.transform.localPosition += new Vector3(-swipeValue, -swipeValue, 0) * Time.deltaTime;
        //        } else if (DownRightSwipe(_currentSwipe)) {
        //            spawnedObject.transform.right += new Vector3(swipeValue, 0, 0) * Time.deltaTime;
        //            spawnedObject.transform.up += new Vector3(0, -swipeValue, 0) * Time.deltaTime;
        //            //spawnedObject.transform.localPosition += new Vector3(swipeValue, -swipeValue, 0) * Time.deltaTime;
        //        }
        //    }
        //    //spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, spawnedObject.transform.localPosition.y, Constant.fixedZPos);
        //    //Debug.Log("Swipe Ended");
        //}

        //private bool LeftSwipe(Vector2 Swipe) {
        //    return _currentSwipe.x < 0 && _currentSwipe.y < 0.5f && _currentSwipe.y > -0.5f;
        //}

        //private bool RightSwipe(Vector2 Swipe) {
        //    return _currentSwipe.x > 0 && _currentSwipe.y < 0.5f && _currentSwipe.y > -0.5f;
        //}

        //private bool UpLeftSwipe(Vector2 Swipe) {
        //    return _currentSwipe.y > 0 && _currentSwipe.x < 0f;
        //}

        //private bool UpRightSwipe(Vector2 Swipe) {
        //    return _currentSwipe.y > 0 && _currentSwipe.x > 0f;
        //}

        //private bool DownLeftSwipe(Vector2 Swipe) {
        //    return _currentSwipe.y < 0 && _currentSwipe.x < 0f;
        //}

        //private bool DownRightSwipe(Vector2 Swipe) {
        //    return _currentSwipe.y < 0 && _currentSwipe.x > 0f;
        //}

        #endregion SwipeLogic

        #region Test Dimension Panel

        [SerializeField] private TMPro.TMP_InputField _inputField_Width;
        [SerializeField] private TMPro.TMP_InputField _inputField_Height;
        [SerializeField] private TMPro.TMP_InputField _inputField_Unit;
        //[SerializeField] private TMPro.TMP_InputField _inputField_Speed;

        public void ResizeFrame() {
            int w = int.Parse(_inputField_Width.text);
            int h = int.Parse(_inputField_Height.text);
            string u = _inputField_Unit.text.ToLower();
            spawnedObject.GetComponent<ARFrame3D>().ResizeWithUnits(w, h, u);
        }

        public void ChangeSpeed(string s) {
        moveSpeed = float.Parse(s);
        }

        #endregion Test Dimension Panel
    }
}