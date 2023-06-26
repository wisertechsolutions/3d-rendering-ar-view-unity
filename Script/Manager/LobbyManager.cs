using AsImpL;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViitorCloud.ModelViewer;

namespace ViitorCloud.ARModelViewer {
    public class LobbyManager : MonoBehaviour {
        public static LobbyManager instance;
        public enum URL_type { Obj, Gltf };
        public URL_type uRL_Type;
        [SerializeField] private GameObject panelLoader;
        [SerializeField] private GameObject panelDark;
        [SerializeField] private TMP_Text txtLoading;
        [SerializeField] private UIManager uIManager;

        private string Url {
            get {
                if (uRL_Type == URL_type.Obj) {
                    return "https://wazir-ai.s3.us-east-2.amazonaws.com/76dd46533464b27512a03ffa4b067319a419493a5a50737287991d008bba6169+(1).zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/fb7a8a82058518326afe01d34139beca358f8fd075895130306fac36bf84b8bb+(1).zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/c973f57c056f6283e8c79357068f5523e0b4857766b88d87285489c92b899036.zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/jet_221fcea2e21bc68674cf779726f2e2b514c35f7d67f151040f6786754af51ea9.zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/statue_0f56432b14d9899a1dc45770744f91f3c58ea1f6380ec786d9a9c6b69770165a.zip";
                } else {
                    //return "https://archive.org/download/paravti/paravti.glb";
                    return "https://wazir-ai.s3.us-east-2.amazonaws.com/it20000-mc512-output.glb";
                }
            }
        }
        private void Awake() {
            instance = this;
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
            panelLoader.SetActive(false);
            model.AddComponent<DontDestroyManager>();
            DataForAllScene.Instance.model3d = model;
            //objParent.ResetPositionAndChildAlignment();
        }

        public void AfterGetURL(string url) {
            if (string.IsNullOrEmpty(url)) {
                return;
            } else {
                panelLoader.SetActive(true);
                uIManager.DownloadorLoadAsset(url, LoadingInProgress);
            }
        }

        private void LoadingInProgress(float obj) {
            txtLoading.text = (obj * 100f).ToString("F1");
        }

        public void LoadScene(int no) {
            panelDark.SetActive(true);
            if (no == 0) {
                SceneManager.LoadScene("Main-AR");
            } else {
                SceneManager.LoadScene("Main-NonAR");
            }
        }
    }
}
