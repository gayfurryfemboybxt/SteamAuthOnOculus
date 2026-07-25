using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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
}
