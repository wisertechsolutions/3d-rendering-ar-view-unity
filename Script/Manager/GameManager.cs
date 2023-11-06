using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ViitorCloud.ARModelViewer {

    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        public GameObject btnSpawnAR;
        public GameObject btnToggles;
        public Rotate3dGameObject objParent;
        public bool touchStart;
        public bool arMode;
        public GameObject btnTouchOnOff;
        public GameObject panelScanFloor;
        public GameObject panelZoomInOut;
        public GameObject panelTapToPlaceObject;

        private void Awake() {
            instance = this;
            EnhancedTouch.TouchSimulation.Enable();
            EnhancedTouch.EnhancedTouchSupport.Enable();
            StartCoroutine(CheckAvailability());
        }

        private void OnEnable() {
            DataForAllScene.Instance.model3d.transform.SetParent(objParent.transform);
            objParent.ResetPositionAndChildAlignment();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                CallPreviousSceneOfNative();
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

        public void TouchOnOffClicked() {
            touchStart = !touchStart;
        }

        public IEnumerator CheckAvailability() {
            if ((ARSession.state == ARSessionState.None) ||
                (ARSession.state == ARSessionState.CheckingAvailability)) {
                yield return ARSession.CheckAvailability();
            }

            Debug.Log($"Current AR Session State:{ARSession.state}");
            //#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            //            if (ARSession.state != ARSessionState.Unsupported) {
            //                if (!PermissionManager.instance.IsCameraPermissionhied()) {
            //                    PermissionManager.instance.RequestCameraPermission();
            //                }
            //            } else if (ARSession.state != ARSessionState.Unsupported) {
            //                btnArOnOff.SetActive(false);
            //                btnSpawnAR.SetActive(false);
            //            }
            //#endif
        }

        public void OnBackButtonPress() {
            CallPreviousSceneOfNative();
        }

        //https://archive.org/download/paravti/paroot.glb
        //https://archive.org/download/paravti/parrotlady.glb
        //https://archive.org/download/paravti/paravti.glb
        //https://archive.org/download/paravti/ardhnarishwar.glb
        //https://archive.org/download/dowry_chest/new%20Matku.glb
        //https://archive.org/download/dowry_chest/dowry_chest.glb
        //https://archive.org/download/dowry_chest/asian_pirate_ship.glb
    }
}