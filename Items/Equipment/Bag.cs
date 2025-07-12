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

using P = Plugin;

public record ItemSize(int Width = 2, int Height = 2) {
  public Vector2int ToVector2int() => new(Width, Height);
}

public record BagBonus(int InvRows = 0, int InvCols = 0);

public record BagData(
  string TechType,
  string Name,
  string Description,
  string IconPath,
  ItemSize ItemSize,
  BagBonus Bonus,
  RecipeData Recipe
);

public class Bag {
  public CustomPrefab Prefab { get; }
  public CraftingGadget CraftingGadget { get; }
  public BagBonus Bonus { get; }
  public TechType TechType => Prefab.Info.TechType;

  public Bag(string techType, string name, string desc, Atlas.Sprite icon, ItemSize itemSize, BagBonus bonus, RecipeData recipe) {
    var prefab = new CustomPrefab(techType, name, desc, icon);
    prefab.Info.WithSizeInInventory(itemSize.ToVector2int());

    var template = new CloneTemplate(prefab.Info, TechType.LuggageBag) { ModifyPrefab = ConfigurePrefab };

    prefab.SetGameObject(template);
    prefab.SetEquipment(EquipmentType.Gloves);

    var craftingGadget = prefab.SetRecipe(recipe);
    craftingGadget.WithFabricatorType(CraftTree.Type.Fabricator)
      .WithStepsToFabricatorTab("Personal", "Equipment").WithCraftingTime(5f);

    prefab.Register();
    prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
    KnownTechHandler.UnlockOnStart(prefab.Info.TechType);

    Prefab = prefab;
    CraftingGadget = craftingGadget;
    Bonus = bonus;
  }

  private void ConfigurePrefab(GameObject go) {
    UnityEngine.Object.DestroyImmediate(go.GetComponentInChildren<PickupableStorage>());
    UnityEngine.Object.DestroyImmediate(go.GetComponentInChildren<PlaceTool>());
    UnityEngine.Object.Destroy(go.GetComponentInChildren<StorageContainer>());

    var vfx = go.FindChild("model").AddComponent<VFXFabricating>();
    vfx.localMinY = -0.1f;
    vfx.localMaxY = 0.4f;
    vfx.posOffset = new Vector3(0.0f, 0.0f, 0.0f);
    vfx.eulerOffset = new Vector3(0.0f, 0.0f, 0.0f);
    vfx.scaleFactor = 0.6f;
  }
}

public class BagRegistrar {
  public static bool Deserialize(string path, out BagData data) {
    data = default!;

    try {
      var obj = JsonConvert.DeserializeObject<BagData>(File.ReadAllText(path), new CustomEnumConverter());
      if (obj is null) {
        P.Logger.LogError($"Failed to deserialize bag data from {path}");
        return false;
      }

      data = obj;
      return true;
    } catch (Exception ex) {
      P.Logger.LogError($"Failed to read bag data from {path}: {ex.Message}");
      return false;
    }
  }

  public static void RegisterBags() {
    foreach (var file in Directory.GetFiles(P.AssetsPath, "*.json", SearchOption.TopDirectoryOnly)) {
      if (!Deserialize(file, out var data)) { continue; }

      var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(P.AssetsPath, data.IconPath), TextureFormat.BC7);
      if (sprite == null) {
        P.Logger?.LogError($"Failed to load icon for bag: {data.Name} from {data.IconPath}");
        continue;
      }

      var bag = new Bag(data.TechType, data.Name, data.Description, sprite, data.ItemSize, data.Bonus, data.Recipe);
      P.Bags.Add(bag.TechType, bag);

      P.Logger?.LogInfo($"Registered bag: {data.Name}, TechType: {data.TechType}");
    }
  }
}
