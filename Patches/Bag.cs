namespace efInventory.Patches;

using HarmonyLib;

using P = Plugin;
using C = Constants;

[HarmonyPatch]
public class BagPatches {

  [HarmonyPrefix]
  [HarmonyPatch(typeof(Equipment), nameof(Equipment.AllowedToAdd))]
  public static bool EquipmentAllowedToAddPrefix(Equipment? __instance, string slot, Pickupable pickupable, bool verbose, ref bool __result) {
    if (__instance == null) { return true; }

    var isBag = P.Bags.ContainsKey(pickupable.GetTechType());
    var isBagSlot = slot.Equals(C.SLOT_NAME);

    if (!isBag && !isBagSlot) { return true; }

    __result = isBag == isBagSlot;
    return false;
  }

  // [HarmonyPrefix]
  // [HarmonyPatch(typeof(uGUI_EquipmentSlot), nameof(uGUI_EquipmentSlot.MarkCompatible))]
  //   public static bool EquipmentSlotMarkCompatiblePrefix(uGUI_EquipmentSlot? __instance, bool state, ref bool __result) {
  //       if (__instance == null) { return true; }

  //       var isBagSlot = __instance.slot.Equals(C.SLOT_NAME);
  //       if (!isBagSlot) { return true; }

  //       __result = true;
  //       return false;
  //   }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnEquip))]
  public static void EquipmentOnEquipPostfix(uGUI_Equipment? __instance, string slot, InventoryItem? item) {
    if (__instance == null || item == null || slot != C.SLOT_NAME) { return; }
    var techType = item.item.GetTechType();

    if (!P.Bags.TryGetValue(techType, out var bag)) { return; }
    var bonus = bag.Bonus;
    if (bonus.InvWidth == 0 && bonus.InvHeight == 0) { return; }

    Inventory.main.container.Resize(Inventory.main.container.sizeX + bonus.InvWidth, Inventory.main.container.sizeY + bonus.InvHeight);
  }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnUnequip))]
  public static void EquipmentOnUnequipPostfix(uGUI_Equipment? __instance, string slot, InventoryItem? item) {
    if (__instance == null || item == null || slot != C.SLOT_NAME) { return; }
    var techType = item.item.GetTechType();

    if (!P.Bags.TryGetValue(techType, out var bag)) { return; }
    var bonus = bag.Bonus;
    if (bonus.InvWidth == 0 && bonus.InvHeight == 0) { return; }

    Inventory.main.container.Resize(Inventory.main.container.sizeX - bonus.InvWidth, Inventory.main.container.sizeY - bonus.InvHeight);
  }
}
