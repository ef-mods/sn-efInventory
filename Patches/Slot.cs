namespace efInventory.Patches;

using HarmonyLib;
using UnityEngine;

using P = Plugin;
using C = Constants;

[HarmonyPatch]
public class SlotPatches {
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Inventory), nameof(Inventory.UnlockDefaultEquipmentSlots))]
  public static void InventoryUnlockDefaultEquipmentSlotsPostfix(Inventory? __instance) {
    if (__instance == null) {
      P.Logger?.LogWarning("Inventory is null, cannot add equipment slot.");
      return;
    }

    __instance.equipment.AddSlot(C.SLOT_NAME);
    P.Logger?.LogInfo($"Added equipment slot: {C.SLOT_NAME}.");
  }

  [HarmonyPrefix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Awake))]
  public static void EquipmentAwakePrefix(uGUI_Equipment? __instance) {
    if (__instance == null) { return; }

    foreach (var component in __instance.GetComponentsInChildren<uGUI_EquipmentSlot>()) {
      if (component == null) { continue; }
      if (!component.slot.Equals("Gloves")) { continue; }

      var slot = Object.Instantiate(component, component.transform.parent);
      if (slot == null) {
        P.Logger?.LogError("Failed to instantiate Gloves slot.");
        break;
      }

      slot.slot = C.SLOT_NAME;
      slot.name = C.SLOT_NAME;
      slot.transform.localPosition = new Vector3(133f, -228f, 0.0f);
      slot.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
      break;
    }
  }
}
