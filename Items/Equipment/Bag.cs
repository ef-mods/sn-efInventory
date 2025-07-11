namespace efInventory.Items.Equipment;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Json.Converters;
using Nautilus.Utility;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public record BagBonus(int InvWidth = 0, int InvHeight = 0);

public record BagSize(int Width = 2, int Height = 2) {
  public Vector2int ToVector2int() => new(Width, Height);
}

public record BagData(
  string TechType,
  string Name,
  string Description,
  string IconPath,
  BagSize ItemSize,
  BagBonus Bonus,
  RecipeData Recipe
);

public record Bag(CustomPrefab Prefab, CraftingGadget CraftingGadget, BagBonus Bonus) {
  public TechType TechType => Prefab.Info.TechType;

  public void Register() {
    CraftingGadget.WithFabricatorType(CraftTree.Type.Fabricator)
      .WithStepsToFabricatorTab("Personal", "Equipment").WithCraftingTime(5f);

    Prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
    Prefab.Register();
    KnownTechHandler.UnlockOnStart(TechType);
  }
}

public class BagRegistrar {
  public static bool Deserialize(string? path, out BagData data) {
    data = default!;

    try {
      var obj = JsonConvert.DeserializeObject<BagData>(File.ReadAllText(path), new CustomEnumConverter());
      if (obj is null) {
        Plugin.Logger?.LogError($"Failed to deserialize bag data from {path}");
        return false;
      }

      data = obj;
      return true;
    } catch (Exception ex) {
      Plugin.Logger?.LogError($"Failed to read bag data from {path}: {ex.Message}");
      return false;
    }
  }

  public static void ConfigurePrefab(GameObject go) {
    UnityEngine.Object.DestroyImmediate(go.GetComponentInChildren<PickupableStorage>());
    UnityEngine.Object.DestroyImmediate(go.GetComponentInChildren<PlaceTool>());
    UnityEngine.Object.Destroy(go.GetComponentInChildren<StorageContainer>());

    var vfxFab = go.FindChild("model").AddComponent<VFXFabricating>();
    vfxFab.localMinY = -0.1f;
    vfxFab.localMaxY = 0.4f;
    vfxFab.posOffset = new Vector3(0.0f, 0.0f, 0.0f);
    vfxFab.eulerOffset = new Vector3(0.0f, 0.0f, 0.0f);
    vfxFab.scaleFactor = 0.6f;
  }

  public static void RegisterBags() {
    foreach (var file in Directory.GetFiles(Plugin.AssetsPath, "*.json", SearchOption.TopDirectoryOnly)) {
      if (!Deserialize(file, out var data)) { continue; }

      var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Plugin.AssetsPath, data.IconPath), TextureFormat.BC7);
      if (sprite == null) {
        Plugin.Logger?.LogError($"Failed to load icon for bag: {data.Name} from {data.IconPath}");
        continue;
      }

      var prefab = new CustomPrefab(data.TechType, data.Name, data.Description, sprite);
      prefab.Info.WithSizeInInventory(data.ItemSize.ToVector2int());

      // var template = new CloneTemplate(prefab.Info, TechType.LuggageBag) { ModifyPrefab = ConfigurePrefab };
      var template = new CloneTemplate(prefab.Info, TechType.LuggageBag);
      template.ModifyPrefab += (Action<GameObject>)(obj => {
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object)obj.GetComponentInChildren<PickupableStorage>());
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object)obj.GetComponentInChildren<PlaceTool>());
        UnityEngine.Object.Destroy((UnityEngine.Object)obj.GetComponentInChildren<StorageContainer>());
        VFXFabricating vfxFabricating = obj.FindChild("model").AddComponent<VFXFabricating>();
        vfxFabricating.localMinY = -0.1f;
        vfxFabricating.localMaxY = 0.4f;
        vfxFabricating.posOffset = new Vector3(0.0f, 0.0f, 0.0f);
        vfxFabricating.eulerOffset = new Vector3(0.0f, 0.0f, 0.0f);
        vfxFabricating.scaleFactor = 0.6f;
      });

      prefab.SetGameObject(template);
      prefab.SetEquipment(EquipmentType.Gloves);

      var craftingGadget = prefab.SetRecipe(data.Recipe);

      var bag = new Bag(prefab, craftingGadget, data.Bonus);
      bag.Register();

      Plugin.Bags.Add(bag.TechType, bag);
      Plugin.Logger?.LogInfo($"Registered bag: {bag.TechType} - {data.Name}");
    }
  }
}
