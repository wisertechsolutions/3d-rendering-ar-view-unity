using UnityEngine;
using ViitorCloud.ARModelViewer;

public class UnityReceiver : MonoBehaviour {

    void Start() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        MyBroadcastReceiver receiver = new MyBroadcastReceiver();
        receiver.Register(context);
    }

    public class MyBroadcastReceiver : AndroidJavaProxy {
        private static readonly string ACTION = "com.example.MY_UNITY_ACTION";

        public MyBroadcastReceiver() : base("android.content.BroadcastReceiver") { }

        public void Register(AndroidJavaObject context) {
            AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter");
            intentFilter.Call("addAction", ACTION);
            context.Call("registerReceiver", this, intentFilter);
        }

        void onReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            string message = intent.Call<string>("getStringExtra", "message");
            Debug.Log("Received message in unity : " + message);

            // Call Unity method here
            //CallUnityMethod(message);
            
        }
        void CallUnityMethod(string message) {
            // Example method call in Unity
            Debug.Log("Calling Unity method with message: " + message);
            // Implement your method call logic here
            GetImageDownloadLink(message);
        }

        public void GetImageDownloadLink(string jsonData) {
            Debug.Log(jsonData);
            ImageData imageData = JsonUtility.FromJson<ImageData>(jsonData);
            DataForAllScene.Instance.isFrameImage = true;
            DataForAllScene.Instance.imageDimensions = new Vector2(imageData.width, imageData.height);
            DataForAllScene.Instance.imageDimensionUnit = imageData.unit.ToLower();
            LobbyManager.instance.DownloadImageCall(imageData.url);
        }

    }
}
