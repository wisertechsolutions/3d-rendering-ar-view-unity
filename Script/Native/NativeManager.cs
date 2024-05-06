using UnityEngine;
namespace ViitorCloud.ARModelViewer {

    public class NativeManager : MonoBehaviour {
        private void AfterUnityLoad() {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("CallMethodToGetURL");
#endif
        }

        private void OnBackToNativeClicked() {
            //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //jo.Call("onBackPressed");
            Application.Quit();
        }

        //public void GetModelDownloadLink(string url, bool isAR) {
        public void GetModelDownloadLinkAR(string url) {
            DataForAllScene.Instance.isAR = true;
            LobbyManager.instance.AfterGetURL(url);
            // Send data to PlaceObject.cs method ARCameraOnOff();
        }

        public void GetModelDownloadLink360(string url) {
            DataForAllScene.Instance.isAR = false;
            LobbyManager.instance.AfterGetURL(url);
            // Send data to PlaceObject.cs method ARCameraOnOff();
        }

        //public void GetImageDownloadLink(string url,float width,float height, string unit) {
        //    DataForAllScene.Instance.isFrameImage = true;
        //    DataForAllScene.Instance.imageDimensions = new Vector2(width,height);
        //    DataForAllScene.Instance.imageDimensionUnit= unit.ToLower();
        //    LobbyManager.instance.DownloadImageCall(url);
        //}
        public void GetImageDownloadLink(string jsonData) {
            Debug.Log(jsonData);
            ImageData imageData = JsonUtility.FromJson<ImageData>(jsonData);
            DataForAllScene.Instance.isFrameImage = true;
            DataForAllScene.Instance.imageDimensions = new Vector2(imageData.width, imageData.height);
            DataForAllScene.Instance.imageDimensionUnit = imageData.unit.ToLower();
            LobbyManager.instance.DownloadImageCall(imageData.url);
        }
    }
    [System.Serializable]
    public class ImageData {
        public string url;
        public float width;
        public float height;
        public string unit;
    }
}

///usefull links
///https://stackoverflow.com/questions/7754373/how-to-call-previous-android-activity-from-unity-activity
///https://stackoverflow.com/questions/21206615/cant-pass-back-event-from-unity-to-android-library-jar
///https://medium.com/@angelhiadefiesta/integrate-a-unity-module-to-a-native-android-app-87644fe899e0
///https://answers.unity.com/questions/780406/androidunity-launching-activity-from-unity-activit.html
///https://medium.com/@davidbeloosesky/embedded-unity-within-android-app-7061f4f473a
///https://www.youtube.com/watch?v=sf54tOAkmzU // perfect video
///https://medium.com/xrpractices/embedding-unity-app-inside-native-android-application-c7b82245f8af