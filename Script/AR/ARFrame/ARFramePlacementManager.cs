using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

        [Header("AR Material")]
        [SerializeField] private Material m_ARPlaneMaterial;
        [SerializeField] private Color m_ARPlaneMaterial_DisableColor;
        [SerializeField] private Color m_ARPlaneMaterial_EnableColor;

        private int colorTempCount = 0;

        private bool waitingLoaderIsOn;
        private Vector3 resetPosition;
        private Quaternion resetRotation;

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

        }

        private void Start() {

            if (mainCam == null) {
                mainCam = Camera.main.transform;
            }
            if (!Input.gyro.enabled) {
                Input.gyro.enabled = true;
            }
            arPlaneManager.planesChanged += OnPlanesChanged;

            SetupInitial();
        }

        private void SetupInitial() {
            waitingLoaderIsOn = true;
            spawnedObject = null;
            planeDetectionCanvas.SetActive(true);
            tapToPlace.SetActive(false);
            StartCoroutine(WaitingPanelHandler());
            m_ARPlaneMaterial.color = m_ARPlaneMaterial_EnableColor;
        }

        private IEnumerator WaitingPanelHandler() {
            yield return new WaitForSeconds(1);
            waitingPanel.SetActive(false);
            waitingLoaderIsOn = false;
        }

        void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs) {
            if (spawnedObject == null && !waitingLoaderIsOn) {
                if (arPlaneManager.trackables.count > 0) {
                    //Debug.Log("AR Plane Detected");
                    planeDetectionCanvas.SetActive(false);
                    tapToPlace.SetActive(true);
                } else {
                    planeDetectionCanvas.SetActive(true);
                    tapToPlace.SetActive(false);
                }
            }

        }


        private void Update() {

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
                        Swipe();
                    }
                    //placementUpdate.Invoke();

                }
            }
        }

        private void SpawnFrame(Pose hitPose) {
            spawnedObject = Instantiate(m_PlacedPrefab);
            SetFrameTransform(hitPose);

            resetPosition = spawnedObject.transform.position;
            resetRotation = spawnedObject.transform.rotation;
            SetFrameData();
            Invoke(nameof(MakeARPlaneTransparent), 1);
        }

        private void MakeARPlaneTransparent() {
            m_ARPlaneMaterial.color = m_ARPlaneMaterial_DisableColor;
        }
        private void SetFrameTransform(Pose hitPose) {
            spawnedObject.transform.position = hitPose.position;
            float dotProduct = Vector3.Dot(-hitPose.up, Camera.main.transform.forward);

            Quaternion rotation = Quaternion.LookRotation(dotProduct > 0 ? -hitPose.up : hitPose.up, Vector3.up);
            spawnedObject.transform.rotation = rotation;
        }
        private void SetFrameData() {
            spawnedObject.GetComponent<ARFrame3D>().DataToDisplay(DataForAllScene.Instance.TextureForFrame, colorFrame[colorTempCount]);
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
            spawnedObject.GetComponent<ARFrame3D>().FrameColorChange(colorFrame[index]);
            colorTempCount = index;
            SelectedColorTickOnOff(colorTempCount);
        }

        public void OnButtonFrameRotate() {
            spawnedObject.GetComponent<ARFrame3D>().RotateTheImage();
        }

        public void OnBackButtonPress() {
            Debug.Log("Back Button Pressed");
            CallPreviousSceneOfNative();
        }

        public void OnButtonReset() {
            //ResetPositionRottion();
            RemoveFrame();
        }

        private void RemoveFrame() {
            if (spawnedObject != null) {
                //DestroyImmediate(spawnedObject);
                //spawnedObject = null;
                //RemoveAllDetectedPlanes();
                ////Reopen UI
                //SetupInitial();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        //private void RemoveAllDetectedPlanes() {
        //    arPlaneManager.SetTrackablesActive(false);
        //    //foreach (var plane in arPlaneManager.trackables) {
        //    //    DestroyImmediate(plane.gameObject);
        //    //}
        //}

        private void ResetPositionRottion() {
            if (spawnedObject != null) {
                spawnedObject.transform.position = resetPosition;
                spawnedObject.transform.rotation = resetRotation;
                spawnedObject.GetComponent<ARFrame3D>().ResetRotation();
            }
        }

        private void CallPreviousSceneOfNative() {
#if UNITY_ANDROID
            // Get the current activity
            //IntPtr jclass = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
            //IntPtr jactivity = AndroidJNI.GetStaticObjectField(jclass, AndroidJNI.GetStaticFieldID(jclass, "currentActivity", "Landroid/app/Activity;"));

            //// Get the finish() method and call it on the current activity
            //IntPtr jmethod = AndroidJNI.GetMethodID(jclass, "finish", "()V");
            //AndroidJNI.CallVoidMethod(jactivity, jmethod, new jvalue[] { });

            //// Release references to avoid memory leaks
            //AndroidJNI.DeleteLocalRef(jactivity);
            //AndroidJNI.DeleteLocalRef(jclass);
            StartCoroutine(CloseActivity());

#elif UNITY_IOS
            Application.Unload();
#endif
        }
        public IEnumerator CloseActivity() {
            Debug.Log("-- close activity start--");
            arPlaneManager.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            //currentActivity.Call("finish");
            CallBackFromNative();
            Debug.Log("-- close activity finished--");
        }
        private void CallBackFromNative() {
            // Find the Java class by its package name and class name
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.gallerie.dashboard.product.activity.ProductDetailsActivity");
            AndroidJavaObject currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the method on the Java object
            currentActivity.Call("onUnityBackPressed");
        }



        #region SwipeLogic

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
                        FixFrameOnWallAR();
                    }
                }
            }
        }

        private void FixFrameOnWallAR() {
            Vector3 dir = spawnedObject.transform.position - Camera.main.transform.position;

            Ray ray = new Ray(Camera.main.transform.position, dir.normalized);

            if (arRaycastManager.Raycast(ray, s_Hits, TrackableType.PlaneWithinPolygon)) {
                Debug.Log("Ray Hit");
                Pose hitPose = s_Hits[0].pose;
                SetFrameTransform(hitPose);
            }
        }

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