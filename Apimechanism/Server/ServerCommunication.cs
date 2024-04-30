using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ViitorCloud.API.StandardTemplates;

namespace ViitorCloud.API {
    /// <summary>
    /// This class is responsible for handling REST API requests to remote server.
    /// To extend this class you just need to add new API methods.
    /// </summary>
    public class ServerCommunication : PersistentLazySingleton<ServerCommunication> {
        #region [Server Communication]

        private readonly bool debug = true;
        public static string CulumusToken = "";
        public static string ViitorCloudToken = "";

        /// <summary>
        /// This method request post method .
        /// </summary>
        /// <param name="form">Data send from local in JSON format.</param>
        /// <param name="url">URL for post method</param>
        /// <param name="callbackOnSuccess">Callback on success.</param>
        /// <param name="callbackOnFail">Callback on fail.</param>
        public void SendRequestPost<T>(string form, string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            StartCoroutine(RequestCoroutinePost(form, url, callbackOnSuccess, callbackOnFail));
        }

        /// <summary>
        /// This method request delete method .
        /// </summary>
        /// <param name="url">URL for delete method</param>
        /// <param name="callbackOnSuccess">Callback on success.</param>
        /// <param name="callbackOnFail">Callback on fail.</param>
        public void SendRequestDelete(string url, UnityAction callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            StartCoroutine(RequestCoroutineDelete(url, callbackOnSuccess, callbackOnFail));
        }

        /// <summary>
        /// This method request get method .
        /// </summary>
        /// <param name="url">URL for get method</param>
        /// <param name="callbackOnSuccess">Callback on success.</param>
        /// <param name="callbackOnFail">Callback on fail.</param>
        public void SendRequestGet<T>(string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            StartCoroutine(RequestCoroutineGet(url, callbackOnSuccess, callbackOnFail));
        }

        /// <summary>
        /// This method request Put method .
        /// </summary>
        /// <param name="form">Data send from local in JSON format.</param>
        /// <param name="url">URL for put method</param>
        /// <param name="callbackOnSuccess">Callback on success.</param>
        /// <param name="callbackOnFail">Callback on fail.</param>
        public void SendRequestPut<T>(string form, string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            StartCoroutine(RequestCoroutinePut(form, url, callbackOnSuccess, callbackOnFail));
        }

        private IEnumerator RequestCoroutinePost<T>(string jsonData, string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            if (debug) {
                Debug.Log("url " + url + " jsonData " + jsonData);
            }
            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData)) {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.method = "POST";
                SetHeader(request);
                yield return request.SendWebRequest();
                SendResponseToAPIMethod(request, url, callbackOnSuccess, callbackOnFail);
            }
        }

        private IEnumerator RequestCoroutinePut<T>(string jsonData, string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            if (debug) {
                Debug.Log("url " + url + " jsonData " + jsonData);
            }
            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData)) {
                request.method = UnityWebRequest.kHttpVerbPUT;
                request.method = "PUT";
                SetHeader(request);
                yield return request.SendWebRequest();
                SendResponseToAPIMethod(request, url, callbackOnSuccess, callbackOnFail);
            }
        }

        private IEnumerator RequestCoroutineGet<T>(string url, UnityAction<T> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            if (debug) {
                Debug.Log("url " + url);
            }
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                request.method = UnityWebRequest.kHttpVerbGET;
                request.method = "GET";
                SetHeader(request);
                yield return request.SendWebRequest();
                SendResponseToAPIMethod(request, url, callbackOnSuccess, callbackOnFail);
            }
        }

        private IEnumerator RequestCoroutineDelete(string url, UnityAction callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            if (debug) {
                Debug.Log("url Delete " + url);
            }
            using (UnityWebRequest request = UnityWebRequest.Delete(url)) {
                request.method = UnityWebRequest.kHttpVerbDELETE;
                request.method = "DELETE";
                SetHeader(request);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError) {

                    Debug.LogError("Delete url " + url + " " + request.error);
                    var apiResponse = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                    callbackOnFail?.Invoke(apiResponse.message);

                    if (debug) {
                        Debug.Log("Delete url " + url + " Data " + request.downloadHandler.text);
                    }
                } else {
                    callbackOnSuccess.Invoke();
                }
            }
        }

        private void SetHeader(UnityWebRequest request) {
            request.timeout = 20;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            if (!string.IsNullOrEmpty(ViitorCloudToken)) {
                request.SetRequestHeader("Authorization", "Bearer " + ViitorCloudToken);
            }
        }

        private void SendResponseToAPIMethod<T>(UnityWebRequest request, string url, UnityAction<T> callbackOnSuccess,
            //For responseCode
            //UnityAction<UnityWebRequest> callbackOnFail) {
            UnityAction<string> callbackOnFail) {
            if (request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError) {

                Debug.LogError("url " + url + " error " + request.error + " error code " + request.responseCode + " Data " + request.downloadHandler.text);

                APIResponse apiResponse = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                if (apiResponse != null) {
                    callbackOnFail?.Invoke(apiResponse.message);
                } else {
                    callbackOnFail?.Invoke(request.error);
                }
                //For responseCode
                //callbackOnFail?.Invoke(request);
            } else {
                if (string.IsNullOrEmpty(request.downloadHandler.text)) {
                    Debug.LogError("DownloadHandler text is null");
                } else {
                    if (debug) {
                        Debug.Log("url " + url + " Data " + request.downloadHandler.text);
                    }
                    //ParseResponse(request.downloadHandler.text, callbackOnSuccess);

                    var parsedData = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    callbackOnSuccess?.Invoke(parsedData);
                }
            }
        }

        ///// <summary>
        ///// This method finishes request process and remove $ sign.
        ///// </summary>
        ///// <param name="data">Data received from server in JSON format.</param>
        ///// <param name="callbackOnSuccess">Callback on success.</param>
        ///// <typeparam name="T">Data Model Type.</typeparam>
        //private void ParseResponse<T>(string data, UnityAction<T> callbackOnSuccess) {
        //    data = data.Replace("$oid", "oid");
        //    data = data.Replace("$date", "date");
        //    var parsedData = JsonUtility.FromJson<T>(data);
        //    callbackOnSuccess?.Invoke(parsedData);
        //}
        #endregion [Server Communication]       
    }
}
