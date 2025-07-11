using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace efInventory;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Plugin : BaseUnityPlugin {
  public new static ManualLogSource Logger { get; private set; }

  private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

  private void Awake() {
    Logger = base.Logger;
    Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
    Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
  }
}
