using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ViitorCloud.API.Constants.API;

namespace ViitorCloud.API
{
    public class GameManager : PersistentLazySingleton<GameManager>
    {
        public Server server;
    }
}
