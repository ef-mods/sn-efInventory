namespace efInventory.Patches;

using HarmonyLib;

using C = Constants;
using P = Plugin;
using S = Settings;

[HarmonyPatch]
public class InventoryPatches {
  /// <summary>
  ///   Resizes the inventory container to the configured width and height.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
  public static void InventoryAwakePostfix() {
    if (!S.InvEnabled || (S.InvCols == C.INV_COLS_DEF && S.InvRows == C.INV_ROWS_DEF)) { return; }
    Inventory.main.container.Resize(S.InvCols, S.InvRows);
    P.Logger.LogDebug($"Inventory resized to {S.InvCols}x{S.InvRows}.");
  }
}
