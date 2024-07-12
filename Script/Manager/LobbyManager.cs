using System;
using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViitorCloud.API;
using ViitorCloud.API.StandardTemplates;
using ViitorCloud.ModelViewer;

namespace ViitorCloud.ARModelViewer {

    public class LobbyManager : MonoBehaviour {
        public static LobbyManager instance;

        public enum URL_type {
            Obj, Gltf, Image
        };

        public URL_type uRL_Type;
        [SerializeField] private GameObject panelLoader;
        [SerializeField] private GameObject panelDark;
        [SerializeField] private TMP_Text txtLoading;
        [SerializeField] private UIManager uIManager;
        public bool ifTesting;

        private DownloadManager _downloaManager;

        [SerializeField]
        private DownloadManager downloadManager {
            get {
                if (_downloaManager == null) {
                    _downloaManager = FindObjectOfType<DownloadManager>();
                }
                return _downloaManager;
            }
            set {
                _downloaManager = value;
            }
        }

        private string Url {
            get {
                if (uRL_Type == URL_type.Obj) {
                    return "https://wazir-ai.s3.us-east-2.amazonaws.com/76dd46533464b27512a03ffa4b067319a419493a5a50737287991d008bba6169+(1).zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/fb7a8a82058518326afe01d34139beca358f8fd075895130306fac36bf84b8bb+(1).zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/c973f57c056f6283e8c79357068f5523e0b4857766b88d87285489c92b899036.zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/jet_221fcea2e21bc68674cf779726f2e2b514c35f7d67f151040f6786754af51ea9.zip";
                    //return "https://wazir-ai.s3.us-east-2.amazonaws.com/statue_0f56432b14d9899a1dc45770744f91f3c58ea1f6380ec786d9a9c6b69770165a.zip";
                } else if (uRL_Type == URL_type.Image) {
                    //return "https://3d-model-construction.s3.ap-south-1.amazonaws.com/frame_0000.png"; //Parth S3 Bucket
                    return "https://drive.google.com/uc?export=download&id=1f82_wC78r58w3GGTT8fjyxe2Vn2gT6Nc"; 
                    //return "https://gallerieapi.imgix.net/Product/04-09-2023-0215351-792285.jpeg";
                } else {
                    // return "https://art-image-bucket.s3.amazonaws.com/artifacts3D/models/02.glb"; //Parth Link
                    //return "https://archive.org/download/paravti/paravti.glb";
                    //return "https://cdn-luma.com/e4e69c53efa92b819e54bc4ceb184074d7c5728459c78f33b6f45334889562c0.glb";
                    return "https://cdn-luma.com/00d536b293be5d40f1a76697fc239dd6b5e6fb6ab762757a4802fbe1a70f6089/Turtle_with_Lotus_textured_mesh_glb.glb";
                    //return "https://art-image-bucket.s3.amazonaws.com/artifacts3D/models/07c2b102-c049-42ba-885c-9181b62ef57e.glb";

                    //return "https://art-image-bucket.s3.amazonaws.com/artifacts3D/models/it20000-mc512.glb";
                }
            }
        }

        public float h = 12f;
        public float w = 8f;

        private void Awake() {
            instance = this;
        }

        private void Start() {
            if (ifTesting) {
                if (uRL_Type == URL_type.Image) {
                    DataForAllScene.Instance.imageDimensions = new Vector2(w, h);
                    DataForAllScene.Instance.imageDimensionUnit = "in";
                    DownloadImageCall(Url);
                } else if (uRL_Type == URL_type.Gltf) {
                    DataForAllScene.Instance.isAR = true;
                    AfterGetURL(Url);
                }
            }
            //GetURL();

#if UNITY_ANDROID
            GetDataFromAndroidIntent();
#endif
        }
    
        private void GetDataFromAndroidIntent() {
#if UNITY_ANDROID
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
                if (intent != null) {
                    string value = intent.Call<string>("getStringExtra", "imageData");
                    Debug.Log("Received value from intent: " + value);
                    if (value == "") {
                        Debug.LogError("File is empty OR does not exist");
                        Application.Quit();
                        return;
                    }
                    NativeManager.instance.GetImageDownloadLink(value);
                }
            }
#endif
        }
    
        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;
        }

        private void OnDisable() {
            uIManager.onModelDownloaded -= Get3dObject;
        }

        private void GetURL() {
            Debug.Log("GetURL Called");
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
            model.transform.position = new Vector3(1000, 1000, 1000); // Ravi
            DataForAllScene.Instance.model3d = model;
            if (!ifTesting) {
                Invoke(nameof(InvokeLoadScene), 1f);
            } else {
                panelLoader.SetActive(false);//ravi
            }
        }

        private void InvokeLoadScene() {
            panelLoader.SetActive(false);
            if (DataForAllScene.Instance.isAR) {
                LoadScene(0);
            } else if (DataForAllScene.Instance.isFrameImage) {
                LoadScene(1);
            } else {
                LoadScene(2);
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
            } else if (no == 1) {
                SceneManager.LoadScene("ARFrameShowcase");
            } else {
                SceneManager.LoadScene("Main-NonAR");
            }
        }

        #region Image Download

        public async void DownloadImageCall(string imageURL) {
            DataForAllScene.Instance.isFrameImage = true;
            panelLoader.SetActive(true);

            await DownloadManager.DownloadAssetAsync(imageURL, (response) => {
                string progressText = "100%";
                txtLoading.text = progressText;

                //To Save New Updated Image
                string name = Path.GetExtension(imageURL);
                string path = Application.persistentDataPath + "/tempImage" + name;
                System.IO.File.WriteAllBytes(path, response);

                Texture downloadedImageTexture;

                Sprite downloadedImage = GetByteToSprite(System.IO.File.ReadAllBytes(path), out downloadedImageTexture);
                DataForAllScene.Instance.TextureForFrame = downloadedImageTexture;
                DataForAllScene.Instance.imageForFrame = downloadedImage;
                Invoke(nameof(InvokeLoadScene), 1f);
            }, (errorMessage) => {
                Debug.LogError(errorMessage);
            }, (progress) => {
                string progressText = (progress * 100f).ToString("F1") + "%";
                txtLoading.text = progressText;
                Debug.Log(progressText);
            });
            //StartCoroutine(DownloadImageURL(imageURL));
        }

        private Sprite GetByteToSprite(byte[] imageBytes, out Texture t) {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            t = tex;
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            return sprite;
        }

        #endregion Image Download
    }
}