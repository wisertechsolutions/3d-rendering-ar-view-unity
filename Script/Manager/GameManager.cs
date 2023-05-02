using UnityEngine;
using ViitorCloud.ModelViewer;

namespace ViitorCloud.ARModelViewer {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        public UIManager uIManager;
        public NativeManager nativeManager;
        public PlaceObject placeObject;
        public GameObject loader;
        public Transform nonARParent;

        private void Awake() {
            instance = this;
        }

        private void OnEnable() {
            uIManager.onModelDownloaded += Get3dObject;
        }

        private void OnDisable() {
            uIManager.onModelDownloaded -= Get3dObject;
        }

        void Get3dObject(GameObject model) {
            GameObject obj = Instantiate(model);
            obj.transform.SetPositionAndRotation(nonARParent.position, nonARParent.rotation);
            obj.transform.parent = nonARParent;
            loader.SetActive(false);
        }

        public void AfterGetURL(string url) {
            if (string.IsNullOrEmpty(url)) {
                return;
            } else {
                loader.SetActive(true);
                uIManager.DownloadorLoadAsset(url);
            }
        }
    }
}
