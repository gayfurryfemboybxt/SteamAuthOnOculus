using Steamworks;
using UnityEngine;

namespace SteamAuthOnOculus.Utils
{
    public class SteamManager: MonoBehaviour
    {
        void Update()
        {
            SteamAPI.RunCallbacks();
        }
    }
}