using System;
using System.Collections.Generic;

namespace ViitorCloud.API.StandardTemplates {
    [Serializable]
    public class APIResponse {
        //public string code;
        public string data;
        public string message;
        public string token;
        public string statusCode;
        public string error;
    }

    [Serializable]
    public class APIResponse<T> where T : class {
        //public string code;
        public T data;
        public string message;
        public string token;
        public string statusCode;
        public string error;
    }

    [Serializable]
    public class APIEventListResponse<T> where T : class {
        //public string code;
        public List<T> data;
        public string message;
        public string token;
        public string statusCode;
        public string error;
    }    

    [Serializable]
    public class RequestURLFor3d {
        public string playerID;
    }

    [Serializable]
    public class GetURLFor3d {
        public string url;
    }
}
