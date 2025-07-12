namespace efInventory.Patches;

using HarmonyLib;
using UnityEngine;

using P = Plugin;
using C = Constants;
using System.Linq;

[HarmonyPatch]
public class SlotPatches {
  /// <summary>
  ///   Adds a new equipment slot for bags..
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(Inventory), nameof(Inventory.UnlockDefaultEquipmentSlots))]
  public static void InventoryUnlockDefaultEquipmentSlotsPostfix(Inventory __instance) {
    __instance.equipment.AddSlot(C.SLOT_BAG_NAME);
    P.Logger.LogInfo($"Added equipment slot: {C.SLOT_BAG_NAME}.");
  }

  /// <summary>
  ///   Adds a new equipment slot for bags in the equipment UI.
  /// </summary>
  [HarmonyPrefix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Awake))]
  public static void EquipmentAwakePrefix(uGUI_Equipment __instance) {
    var component = __instance.GetComponentsInChildren<uGUI_EquipmentSlot>().First(c => c.slot.Equals(C.SLOT_GLOVES_NAME));
    var slot = Object.Instantiate(component, component.transform.parent);
    slot.slot = C.SLOT_BAG_NAME;
    slot.name = C.SLOT_BAG_NAME;
    slot.transform.localPosition = new Vector3(133f, -228f, 0.0f);
    slot.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
  }
}
