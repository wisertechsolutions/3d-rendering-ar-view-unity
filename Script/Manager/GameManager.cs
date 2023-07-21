using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

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
            Application.Quit();
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
