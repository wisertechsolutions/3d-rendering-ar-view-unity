using UnityEngine;

namespace ViitorCloud.ARModelViewer {
    public class DataForAllScene : MonoBehaviour {
        public static DataForAllScene Instance { get; private set; }
        public GameObject model3d;
        public bool isAR;

        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else
                Destroy(gameObject);
        }
    }
}
