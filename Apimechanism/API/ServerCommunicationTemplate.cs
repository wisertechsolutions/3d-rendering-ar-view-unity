using UnityEngine.Events;

using ViitorCloud.API.StandardTemplates;

namespace ViitorCloud.API {
    public class ServerCommunicationTemplate : PersistentLazySingleton<ServerCommunicationTemplate> {
        #region [API]     

        /// <summary>
        /// This method call server API to get Login.
        /// </summary>
        /// <param name="callbackOnSuccess">Callback on success.</param>
        /// <param name="callbackOnFail">Callback on fail.</param>
        public void RequestModelURL(string form, UnityAction<APIResponse<GetURLFor3d>> callbackOnSuccess,
            UnityAction<string> callbackOnFail) {
            ServerCommunication.Instance.SendRequestPost(form, Constants.API.Register,
                callbackOnSuccess, callbackOnFail);
        }

        #endregion [API]          
    }
}
