using System.Collections.Generic;
using HarmonyLib;
using SteamAuthOnOculus;
using UnityEngine;

namespace SteamAuthOnOculus.Patches
{
    [HarmonyPatch(typeof(PhotonAuthenticator), "SetCustomAuthenticationParameters")]
    public static class SetCustomAuthenticationParametersPatch
    {
        static void Prefix(ref Dictionary<string, object> customAuthData)
        {
            customAuthData.Remove("Platform"); // steam has none so just remove it
        }
    }
}
