using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ViitorCloud.ARModelViewer {

    public class DownloadManager : MonoBehaviour {

        public static async Task DownloadAssetAsync(string url,
            Action<Byte[]> OnDownloadComplete = null,
            Action<string> OnDownloadFail = null,
            Action<float> progress = null) {
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();

                while (!asyncOp.isDone) {
                    progress?.Invoke(asyncOp.progress);
                    await Task.Delay(100);
                }

                if (request.result != UnityWebRequest.Result.Success) {
                    OnDownloadFail?.Invoke(request.error);
                } else {
                    byte[] data = request.downloadHandler.data;
                    OnDownloadComplete?.Invoke(data);
                }
            }
        }
    }//DownloadManager class end
}