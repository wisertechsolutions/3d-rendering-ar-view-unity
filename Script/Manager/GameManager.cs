using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ViitorCloud.ModelViewer;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TMPro;

namespace ViitorCloud.ARModelViewer {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        [SerializeField] private UIManager uIManager;
        [SerializeField] private NativeManager nativeManager;
        [SerializeField] private PlaceObject placeObject;
        [SerializeField] private GameObject loader;
        [SerializeField] private GameObject btnArOnOff;
        [SerializeField] private GameObject btnSpawnAR;
        [SerializeField] private TMP_Text txtLoading;
        public Rotate3dGameObject nonARParent;
        public Rotate3dGameObject aRParent;
        public bool touchStart;
        public bool arMode = true;

        private void Awake() {
            instance = this;
            EnhancedTouch.TouchSimulation.Enable();
            EnhancedTouch.EnhancedTouchSupport.Enable();
            StartCoroutine(CheckAvailability());
        }

        private void Start() {
            //string url = "https://archive.org/download/paravti/paroot.glb";
            //string url = "https://archive.org/download/paravti/ardhnarishwar.glb";
            string url = "https://archive.org/download/paravti/paravti.glb";
            //string url = "https://archive.org/download/paravti/parrotlady.glb";
            //string url = "https://archive.org/download/dowry_chest/asian_pirate_ship.glb";
            //string url = "https://archive.org/download/dowry_chest/dowry_chest.glb";
            //string url = "https://3d-model-construction.s3.ap-south-1.amazonaws.com/4b6f1d6ce2a511fa3c16f93f4916f2b49583e2744a49bea46bdbb49234e1afdf.zip";
            GameManager.instance.AfterGetURL(url);
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
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (ARSession.state != ARSessionState.Unsupported) {
                if (!PermissionManager.instance.IsCameraPermissionhied()) {
                    PermissionManager.instance.RequestCameraPermission();
                }
            } else if (ARSession.state != ARSessionState.Unsupported) {
                btnArOnOff.SetActive(false);
                btnSpawnAR.SetActive(false);
            }
#endif
        }

        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;
        }

        private void OnDisable() {
            uIManager.onModelDownloaded -= Get3dObject;
        }

        private void OnApplicationFocus(bool focus) {
            if (focus) {
                btnArOnOff.SetActive(PermissionManager.instance.IsCameraPermissionhied());
                btnSpawnAR.SetActive(PermissionManager.instance.IsCameraPermissionhied());
            }
        }

        private void Get3dObject(GameObject model) {
            txtLoading.text = "100.0";
            GameObject obj = Instantiate(model);
            obj.transform.SetPositionAndRotation(nonARParent.transform.position, nonARParent.transform.rotation);
            obj.transform.parent = nonARParent.transform;
            obj.transform.localScale = Vector3.one;
            loader.SetActive(false);

            nonARParent.ResetPositionAndChildAlignment();
            aRParent.ResetPositionAndChildAlignment();
        }

        public void AfterGetURL(string url) {
            if (string.IsNullOrEmpty(url)) {
                return;
            } else {
                loader.SetActive(true);
                uIManager.DownloadorLoadAsset(url, LoadingInProgress);
            }
        }

        private void LoadingInProgress(float obj) {
            //Debug.Log("Download % "+ obj);
            txtLoading.text = (obj * 100f).ToString("F1");
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
