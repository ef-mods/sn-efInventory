namespace efInventory.Patches;

using HarmonyLib;

using P = Plugin;
using S = Settings;
using C = Constants;

[HarmonyPatch]
public class InventoryPatches {
  /// <summary>
  ///   Resizes the inventory container to the configured width and height.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
  public static void InventoryAwakePostfix() {
    if (S.InvWidth == C.INV_DEF_W && S.InvHeight == C.INV_DEF_H) { return; }
    Inventory.main.container.Resize(S.InvWidth, S.InvHeight);
    P.Logger.LogDebug($"Inventory resized to {S.InvWidth}x{S.InvHeight}.");
  }
}
