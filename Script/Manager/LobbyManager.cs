using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViitorCloud.ARModelViewer {
    public class LobbyManager : MonoBehaviour {

        public void LoadScene(int no) {
            if (no == 0) {
                SceneManager.LoadScene("Main-AR");
            } else {
                SceneManager.LoadScene("Main-NonAR");
            }
        }
    }
}
