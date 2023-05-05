using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ViitorCloud.ModelViewer;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TMPro;

namespace ViitorCloud.ARModelViewer {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        public UIManager uIManager;
        public NativeManager nativeManager;
        public PlaceObject placeObject;
        public GameObject loader;
        public Transform nonARParent;
        public TMP_Text txtLoading;

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
            GameManager.instance.AfterGetURL(url);
        }

        public IEnumerator CheckAvailability() {
            if ((ARSession.state == ARSessionState.None) ||
                (ARSession.state == ARSessionState.CheckingAvailability)) {
                yield return ARSession.CheckAvailability();
            }

            Debug.Log($"Current AR Session State:{ARSession.state}");
        }

        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;

        }

        private void OnDisable() {
            //EnhancedTouch.TouchSimulation.Disable();
            uIManager.onModelDownloaded -= Get3dObject;
        }

        void Get3dObject(GameObject model) {
            txtLoading.text = "100.0";
            GameObject obj = Instantiate(model);
            obj.transform.SetPositionAndRotation(nonARParent.position, nonARParent.rotation);
            obj.transform.parent = nonARParent;
            obj.transform.localScale = Vector3.one;
            loader.SetActive(false);
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
            Debug.Log("Download % "+ obj);
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
