using AsImpL;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViitorCloud.ModelViewer;

namespace ViitorCloud.ARModelViewer {
    public class LobbyManager : MonoBehaviour {

        public enum URL_type { Obj, Gltf };
        public URL_type uRL_Type;
        [SerializeField] private GameObject loader;
        [SerializeField] private TMP_Text txtLoading;
        [SerializeField] private UIManager uIManager;

        private string Url {
            get {
                if (uRL_Type == URL_type.Obj) {
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/76dd46533464b27512a03ffa4b067319a419493a5a50737287991d008bba6169+(1).zip";
                    return "https://wazir-ai.s3.us-east-2.amazonaws.com/fb7a8a82058518326afe01d34139beca358f8fd075895130306fac36bf84b8bb+(1).zip";
                } else {
                    return "https://archive.org/download/paravti/paravti.glb";
                }
            }
        }        

        private void Start() {
            AfterGetURL(Url);
        }

        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;
        }

        private void OnDisable() {
            uIManager.onModelDownloaded -= Get3dObject;
        }

        private void Get3dObject(GameObject model) {
            txtLoading.text = "100.0";
            loader.SetActive(false);
            //objParent.ResetPositionAndChildAlignment();
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
            txtLoading.text = (obj * 100f).ToString("F1");
        }

        public void LoadScene(int no) {
            if (no == 0) {
                SceneManager.LoadScene("Main-AR");
            } else {
                SceneManager.LoadScene("Main-NonAR");
            }
        }
    }
}
