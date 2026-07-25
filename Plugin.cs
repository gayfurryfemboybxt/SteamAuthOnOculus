using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks;

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
        Logger.LogWarning("uhh lowke donteven know if it works, theoretically it should, dont exactly know how to get steam to communicate with the mod yet.");

        hm.PatchAll();
    }

    void Start()
    {
        //make a steam_appid txt file
        var path = Path.Combine(Paths.GameRootPath, "steam_appid.txt");
        try
        {
            if (!File.Exists(path))
            {
                var steam_appid = File.Create(path);
                File.WriteAllText(path, "1533390");
            }
        }catch {Logger.LogError("so basically i tried doin it and it ain work");}
        SteamAPI.Init();
    }
}
