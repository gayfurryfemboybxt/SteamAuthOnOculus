using GorillaNetworking;
using HarmonyLib;
using SteamAuthOnOculus.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using System.Globalization;

namespace SteamAuthOnOculus.Patches
{
    [HarmonyPatch(typeof(PlayFabAuthenticator), "BeginLoginFlow")]
    static class BeginLogFlowPatch
    {
        public static HAuthTicket steamAuthTicketPlayFab = HAuthTicket.Invalid;
        public static HAuthTicket steamAuthTicketPhoton = HAuthTicket.Invalid;
        public static string steamAuthIdForPhoton;
        public static PlayFabAuthenticator playfabAuthenticatorInstance;
        public static bool Prefix(PlayFabAuthenticator __instance)
        {
            __instance.platform.PlatformTag = "Steam";
            PlayFabSettings.TitleId = "63FDD";
            playfabAuthenticatorInstance = __instance;
            NetworkSystem.Instance.OnCustomAuthenticationResponse += OnCustomAuthenticationResponse;
            MothershipClientApiUnity.StartLoginWithSteam(delegate (PlayerSteamBeginLoginResponse resp)
            {
                Plugin.Logger.LogInfo("starting mothership steam authentication");
                string nonce = resp.Nonce;
                Plugin.Logger.LogInfo($"got login with steam nonce! {nonce}");
                Utils.SteamAuthTicket ticketHand = HAuthTicket.Invalid;
                ticketHand = SteamHelper.GetAuthTicketForWebApi(nonce, delegate (string ticket)
                {
                    Plugin.Logger.LogInfo("successfully got steam ticket");
                    string nonce2 = nonce; 
                    Action<LoginResponse> onSuccess = successResp =>
                    {
                        ticketHand.Dispose();
                        Plugin.Logger.LogInfo("Logged in to Mothership with Steam");
                        MothershipClientApiUnity.OpenNotificationsSocket();

                        //success!
                        Plugin.Logger.LogInfo("authenticating with playfan now!!!!");
                        __instance.userID = SteamUser.GetSteamID().ToString();
                       
                        steamAuthTicketPlayFab = SteamHelper.GetAuthTicket((string ticket) =>
                        {
                            Plugin.Logger.LogInfo("got steam auth ticket");
                            PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
                            {
                                CreateAccount = true,
                                SteamTicket = ticket
                            }, (LoginResult res) =>
                            {
                                __instance._playFabPlayerIdCache = res.PlayFabId;
                                __instance._sessionTicket = res.SessionTicket;
                                 Plugin.Logger.LogInfo($"Logged in with { __instance._playFabPlayerIdCache}! {MothershipClientContext.MothershipId}");

                                __instance.StartCoroutine(__instance.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
                                {
                                    Platform = __instance.platform.ToString(),
                                    SessionTicket = __instance._sessionTicket,
                                    PlayFabId = __instance._playFabPlayerIdCache,
                                    TitleId = "63FDD",
                                    MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
                                    MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
                                    MothershipToken = MothershipClientContext.Token,
                                    MothershipId = MothershipClientContext.MothershipId
                                }, (PlayFabAuthenticator.CachePlayFabIdResponse res) =>
                                {
                                    if (res != null)
                                    {
                                        steamAuthIdForPhoton = res.SteamAuthIdForPhoton;
                                        DateTime dT;
                                        if (DateTime.TryParse(res.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dT))
                                        {
                                            __instance.StartCoroutine(__instance.VerifyKidAuthenticated(dT));
                                        }
                                        Plugin.Logger.LogInfo($"cached ze playfab id using {res.SteamAuthIdForPhoton}");//advance login
                                        SteamHelper.RefreshSteamAuthTicketForPhoton(__instance, (string ticket) =>
                                        {
                                            __instance._nonce = ticket;
                                            Plugin.Logger.LogInfo($"got nonce!");
                                            __instance.AuthenticateWithPhoton();
                                        }, (EResult res) =>
                                        {
                                            Plugin.Logger.LogInfo($"failed to get nonce");
                                        });
                                        return;
                                    }
                                    Plugin.Logger.LogInfo($"ffailed to cache playfab id");
                                }));


                            }, (PlayFabError err) =>
                            {
                                Plugin.Logger.LogInfo($"failed to authenticate withplayfab {err.ErrorMessage}");
                            });
                        }, (EResult result) =>
                        {
                            Plugin.Logger.LogInfo("uhh what?");
                        });

                    };
                    Action<MothershipError, int> onError = (MothershipError err, int errCode) =>
                    {
                        Plugin.Logger.LogInfo($"failed to complete mothership login {err.Message} {errCode}");
                    };
                    MothershipClientApiUnity.CompleteLoginWithSteam(nonce2, ticket, onSuccess, onError);
                }, (EResult res) =>
                {
                    Plugin.Logger.LogInfo("failed to get steam ticket");
                });
            }, (MothershipError err, int errcode) =>
            {
                Plugin.Logger.LogInfo($"failed to login with mothership {err.Message} {errcode}");
            });
            return false; //prevents original loginflow from running. needed for authenticating with steam.
        }
    
        static void OnCustomAuthenticationResponse(Dictionary<string,object> res) //they stripped this too
        {
            Utils.SteamAuthTicket steamAuthTicket = steamAuthTicketPhoton;
            if (steamAuthTicket != null)
            {
                steamAuthTicket.Dispose();
            }
            object obj;
            if (res.TryGetValue("SteamAuthIdForPhoton", out obj))
            {
                string text = obj as string;
                if (text != null)
                {
                    steamAuthIdForPhoton = text;
                    return;
                }
            }
            steamAuthIdForPhoton = null;
        }
    }
}
