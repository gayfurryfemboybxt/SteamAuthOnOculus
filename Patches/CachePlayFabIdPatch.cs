using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace SteamAuthOnOculus.Patches
{
    [HarmonyPatch(typeof(PlayFabAuthenticator), "CachePlayFabId")]
    static class CachePlayFabIdPatch
    {
        static bool Prefix(PlayFabAuthenticator __instance, PlayFabAuthenticator.CachePlayFabIdRequest data,  Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback, ref IEnumerator __result)
        {
            __result = CachePlayFabId(__instance, data, callback);
            return false;
        }

        static IEnumerator CachePlayFabId(PlayFabAuthenticator instance, PlayFabAuthenticator.CachePlayFabIdRequest data,  Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
        {
            Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse cachePlayFabIdResponse = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback(cachePlayFabIdResponse);
				}
			}
            yield break;
        }
    }
}