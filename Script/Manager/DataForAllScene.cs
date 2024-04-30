using UnityEngine;

namespace ViitorCloud.ARModelViewer {

    public class DataForAllScene : MonoBehaviour {

        public static DataForAllScene Instance {
            get; private set;
        }

        public GameObject model3d;
        public bool isAR;
        public bool isFrameImage;
        public Sprite imageForFrame;
        public Vector2 imageDimensions;
        public string imageDimensionUnit;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else
                Destroy(gameObject);
        }
    }
}