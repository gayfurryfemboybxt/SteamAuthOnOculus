using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace SteamAuthOnOculus;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static Harmony hm;
        
    private void Awake()
    {
        hm = new Harmony(MyPluginInfo.PLUGIN_NAME);
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Logger.LogWarning("make sure you follow the tutorial on github! https://github.com/jrvr-cs/SteamAuthOnOculus");

        Logger.LogInfo($"tryna make steam txt for auth {Paths.GameRootPath}");
        var path = Path.Combine(Paths.GameRootPath, "steam_appid.txt");
        try
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "1533390");
            }
        }catch {Logger.LogError("so basically i tried doin it and it ain work");}
        
        SteamAPI.Init();
        GameObject ss = new GameObject("JRVR.SteamManager");
        ss.AddComponent<Utils.SteamManager>();
        DontDestroyOnLoad(ss);


        hm.PatchAll();
    }

    

}
