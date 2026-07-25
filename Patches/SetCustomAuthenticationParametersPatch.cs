using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PhotonAuthenticator), "SetCustomAuthenticationParameters")]
public static class SetCustomAuthenticationParametersPatch
{
    static bool Prefix(ref Dictionary<string, object> customAuthData)
    {

        customAuthData.Remove("Platform"); //steam authenticates with photon without a platform identifier
        return false;//please we need authenticate steeam!
    }
}