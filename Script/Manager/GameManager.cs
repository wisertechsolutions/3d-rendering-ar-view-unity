using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using System;

namespace ViitorCloud.ARModelViewer {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        public GameObject btnSpawnAR;
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
            //Application.Quit();
#if UNITY_ANDROID
            // Get the current activity
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            // Call the finish method to return to the previous activity
            currentActivity.Call("finish");
#elif UNITY_IOS
// Get a reference to the current navigation controller
IntPtr uiNavigationControllerClass = GetClass("UINavigationController");
IntPtr uiNavigationControllerInstance = ObjC.CallStatic<NSObject>(uiNavigationControllerClass, "visibleViewController").GetRawClass();
// Call the popViewControllerAnimated method to return to the previous view controller
ObjC.Call(uiNavigationControllerInstance, "popViewControllerAnimated:", new object[] { true });
#endif
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
