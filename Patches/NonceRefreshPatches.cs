using SteamAuthOnOculus.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using GorillaNetworking;
using System.Reflection;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

namespace SteamAuthOnOculus.Patches
{
    [HarmonyPatch(typeof(NetworkSystem), "RefreshNonce")]
    public static class RefreshNoncePatch //AA stripped "RefreshSteamAuthTicketForPhoton" so we cant call it normally
    {
        // static MethodBase TargetMethod()
        // {
        //     return AccessTools.Method(typeof(NetworkSystem), "RefreshNonce");
        // }

        static bool Prefix(NetworkSystem __instance)
        {
            __instance.nonceRefreshed = false;
            if (BeginLogFlowPatch.playfabAuthenticatorInstance != null)
            {
                SteamHelper.RefreshSteamAuthTicketForPhoton(BeginLogFlowPatch.playfabAuthenticatorInstance, (string token) =>
                {
                    AuthenticationValues authValues = __instance.GetAuthenticationValues();
                    Dictionary<string, object> dict = (authValues != null ? authValues.AuthPostData : null) as Dictionary<string,object>;
                    if (dict != null)
                    {
                        dict["Nonce"] = token;
                        authValues.SetAuthPostData(dict);
                        __instance.SetAuthenticationValues(authValues);
                        __instance.nonceRefreshed = true;
                    }
                }, null);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(NetworkSystem), "ReGetNonce")]
    public static class ReGetNoncePatch
    {
        // static MethodBase TargetMethod()
        // {
        //     return AccessTools.Method(typeof(NetworkSystem), "ReGetNonce");
        // }

        static bool Prefix(NetworkSystem __instance, ref IEnumerator __result)
        {
            __result = getsteamticket(__instance);
            return false;
        }

        static IEnumerator getsteamticket(NetworkSystem insance)
        {
            yield return new WaitForSecondsRealtime(3f);
            if (BeginLogFlowPatch.playfabAuthenticatorInstance != null)
            {
                SteamHelper.RefreshSteamAuthTicketForPhoton(BeginLogFlowPatch.playfabAuthenticatorInstance, (string token) =>
                {
                    AuthenticationValues authValues = insance.GetAuthenticationValues();
                    Dictionary<string, object> dict = (authValues != null ? authValues.AuthPostData : null) as Dictionary<string,object>;
                    if (dict != null)
                    {
                        dict["Nonce"] = token;
                        authValues.SetAuthPostData(dict);
                        insance.SetAuthenticationValues(authValues);
                        insance.nonceRefreshed = true;
                    }
                }, null);
            }
            yield break;
        }
    }
}
