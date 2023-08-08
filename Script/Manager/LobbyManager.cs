using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViitorCloud.API;
using ViitorCloud.API.StandardTemplates;
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
                    return "https://cdn-luma.com/e4e69c53efa92b819e54bc4ceb184074d7c5728459c78f33b6f45334889562c0.glb";
                }
            }
        }
        private void Awake() {
            instance = this;
        }

        private void Start() {
           // AfterGetURL(Url);
            //GetURL();
        }

        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;
        }

        private void OnDisable() {
            uIManager.onModelDownloaded -= Get3dObject;
        }

        private void GetURL() {
            RequestURLFor3d getURLFor3D = new RequestURLFor3d();
            getURLFor3D.playerID = "fdaf123";
            ServerCommunicationTemplate.Instance.RequestModelURL(JsonUtility.ToJson(getURLFor3D), OnRecieve3dModelURLData, OnDataLoadFail);
        }

        private void OnDataLoadFail(string msg) {
            Debug.Log("Fail");
        }

        private void OnRecieve3dModelURLData(APIResponse<GetURLFor3d> getURLFor3d) {
            Debug.Log("OnRecieve3dModelURLData " + getURLFor3d.data.url);
            AfterGetURL(getURLFor3d.data.url);
        }

        private void Get3dObject(GameObject model) {
            txtLoading.text = "100.0";
            
            model.AddComponent<DontDestroyManager>();
            DataForAllScene.Instance.model3d = model;
            //objParent.ResetPositionAndChildAlignment();
            //Invoke(nameof(InvokeLoadScene),1f);
            panelLoader.SetActive(false);//ravi
        }

        void InvokeLoadScene() {
            panelLoader.SetActive(false);
            if (DataForAllScene.Instance.isAR) {
                LoadScene(0);
            } else {
                LoadScene(1);
            }
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
