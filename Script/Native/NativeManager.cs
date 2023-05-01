using UnityEngine;

public class NativeManager : MonoBehaviour {
    private void Start() {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("CallMethodToGetURL");
    }

    private void OnBackToNativeClicked() {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("onBackPressed");
    }

    public void GetModelDownloadLink(string url) {
        GameManager.instance.AfterGetURL(url);
    }
}

///usefull links
///https://stackoverflow.com/questions/7754373/how-to-call-previous-android-activity-from-unity-activity
///https://stackoverflow.com/questions/21206615/cant-pass-back-event-from-unity-to-android-library-jar
///https://medium.com/@angelhiadefiesta/integrate-a-unity-module-to-a-native-android-app-87644fe899e0
///https://answers.unity.com/questions/780406/androidunity-launching-activity-from-unity-activit.html
///https://medium.com/@davidbeloosesky/embedded-unity-within-android-app-7061f4f473a
///https://www.youtube.com/watch?v=sf54tOAkmzU // perfect video