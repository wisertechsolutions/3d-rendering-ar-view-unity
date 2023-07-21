
namespace ViitorCloud.API.Constants {
    /// <summary>
    /// API response.
    /// </summary>
    public class APIResponse {
        public const string StatusSuccess = "200";
    }

    public class API {
        public static string APIDevelopmentBaseURL = "https://stg-demourl.com/";
        public static string APIProductionBaseURL = "https://prod-demourl.com/";

        public static string Login = APIBaseURL + "users/login";
        public static string Register = APIBaseURL + "users/register";

        public static string APIBaseURL {
            get {
                switch (GameManager.Instance.server) {
                    case Server.Live:
                        return APIProductionBaseURL;
                    case Server.Development:
                        return APIDevelopmentBaseURL;
                }

                return APIProductionBaseURL;
            }
        }

        public enum Server {
            Live,
            Development,
        }
    }
}