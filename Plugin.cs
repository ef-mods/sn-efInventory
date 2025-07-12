using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Collections.Generic;
using efInventory.Items.Equipment;

namespace efInventory;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Plugin : BaseUnityPlugin {
  private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

  public new static ManualLogSource Logger { get; private set; } = null!;
  public static string AssetsPath => Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");

  public static Dictionary<TechType, Bag> Bags { get; } = new();

  private void Awake() {
    Logger = base.Logger;
    Settings.Instance.Load();

    Equipment.slotMapping.Add(Constants.SLOT_BAG_NAME, EquipmentType.Gloves);
    BagRegistrar.RegisterBags();

    Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
    Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
  }
}
