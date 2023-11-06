using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace ViitorCloud.ARModelViewer {
    public class PermissionManager : MonoBehaviour {
        public static PermissionManager instance;

        private void Awake() {
            instance = this;
        }

        public bool IsCameraPermissionhied() {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif
        }

        public void RequestCameraPermission() {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                Permission.RequestUserPermission(Permission.Camera);
            }
#elif UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
#endif
        }
    }
}
