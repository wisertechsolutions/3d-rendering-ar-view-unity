using UnityEngine;

namespace ViitorCloud.ARModelViewer {
    public class DontDestroyManager : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
