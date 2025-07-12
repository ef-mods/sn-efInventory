namespace efInventory.Patches;

using HarmonyLib;
using System.Collections.Generic;

using P = Plugin;
using C = Constants;

[HarmonyPatch]
public class BagPatches {
  // helpers to make the code more readable
  private static bool IsBag(TechType techType) => P.Bags.ContainsKey(techType);
  private static bool IsGloves(TechType techType) => CraftData.GetEquipmentType(techType).Equals(EquipmentType.Gloves) && !IsBag(techType);
  private static bool IsNeitherBagNorGloves(TechType techType) => !IsBag(techType) && !CraftData.GetEquipmentType(techType).Equals(EquipmentType.Gloves);
  private static bool IsBagSlot(string slot) => slot.Equals(C.SLOT_BAG_NAME);
  private static bool IsGlovesSlot(string slot) => slot.Equals(C.SLOT_GLOVES_NAME);

  private static bool IsCorrectBGSlot(TechType techType, string slot)
  => (IsBag(techType) && IsBagSlot(slot)) || (IsGloves(techType) && IsGlovesSlot(slot));

  /// <summary>
  ///   Overrides the default equipment slot check to allow adding bags and gloves inly to their respective slots.
  /// </summary>
  [HarmonyPrefix]
  [HarmonyPatch(typeof(Equipment), nameof(Equipment.AllowedToAdd))]
  public static bool EquipmentAllowedToAddPrefix(string slot, Pickupable pickupable, bool verbose, ref bool __result) {
    if (IsNeitherBagNorGloves(pickupable.GetTechType())) { return true; }
    __result = IsCorrectBGSlot(pickupable.GetTechType(), slot);
    return false;
  }

  /// <summary>
  ///   Disable bag snapping into glove slot and vice versa.
  /// </summary>
  [HarmonyPrefix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnDragHoverStay))]
  public static bool OnDragHoverStayPrefix(string slotB)
  => ItemDragManager.isDragging &&
    (IsNeitherBagNorGloves(ItemDragManager.draggedItem.item.GetTechType()) ||
    IsCorrectBGSlot(ItemDragManager.draggedItem.item.GetTechType(), slotB));

  /// <summary>
  ///   Overrides slot highlighting to only highlight the correct bag or gloves slot when dragging an item.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnItemDragStart))]
  private static void OnItemDragStartPostfix(uGUI_Equipment __instance, Pickupable p) {
    if (IsNeitherBagNorGloves(p.GetTechType())) { return; }
    var slots = (Dictionary<string, uGUI_EquipmentSlot>)AccessTools.Field(typeof(uGUI_Equipment), "allSlots").GetValue(__instance);
    slots.ForEach(kvp => kvp.Value.MarkCompatible(IsCorrectBGSlot(p.GetTechType(), kvp.Value.slot)));
  }

  /// <summary>
  ///   Overrides slot highlighting to only highlight the correct bag or gloves slot when hovering over an item.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnPointerEnter))]
  public static void OnPointerEnterPostfix(uGUI_InventoryTab __instance, InventoryItem item) {
    if (ItemDragManager.isDragging || !Player.main.GetPDA().isInUse || IsNeitherBagNorGloves(item.item.GetTechType())) { return; }
    var slots = (Dictionary<string, uGUI_EquipmentSlot>)AccessTools.Field(typeof(uGUI_Equipment), "allSlots").GetValue(__instance.equipment);
    slots.ForEach(kvp => kvp.Value.MarkCompatible(IsCorrectBGSlot(item.item.GetTechType(), kvp.Value.slot)));
  }

  /// <summary>
  ///   Increases the inventory size by the bonus amount on equipping a bag.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnEquip))]
  public static void EquipmentOnEquipPostfix(string slot, InventoryItem item) {
    if (slot != C.SLOT_BAG_NAME || !P.Bags.TryGetValue(item.item.GetTechType(), out var bag)) { return; }

    var cols = Inventory.main.container.sizeX + bag.Bonus.InvCols;
    var rows = Inventory.main.container.sizeY + bag.Bonus.InvRows;

    Inventory.main.container.Resize(cols, rows);
  }

  /// <summary>
  ///   Decreases the inventory size by the bonus amount on unequipping a bag.
  /// </summary>
  [HarmonyPostfix]
  [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.OnUnequip))]
  public static void EquipmentOnUnequipPostfix(string slot, InventoryItem item) {
    if (slot != C.SLOT_BAG_NAME || !P.Bags.TryGetValue(item.item.GetTechType(), out var bag)) { return; }

    var cols = Inventory.main.container.sizeX - bag.Bonus.InvCols;
    var rows = Inventory.main.container.sizeY - bag.Bonus.InvRows;

    Inventory.main.container.Resize(cols, rows);
  }
}
