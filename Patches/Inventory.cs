namespace efInventory.Patches;

using HarmonyLib;

using P = Plugin;
using S = Settings;
using C = Constants;

[HarmonyPatch]
public class InventoryPatches {
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
  public static void InventoryAwakePostfix(Inventory? __instance) {
    if (__instance == null) { return; }
    if (S.InvWidth == C.DEF_W && S.InvHeight == C.DEF_H) { return; }
    Inventory.main.container.Resize(S.InvWidth, S.InvHeight);
    P.Logger?.LogDebug($"Inventory resized to {S.InvWidth}x{S.InvHeight}.");
  }
}
