namespace efInventory;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

using C = Constants;

[Menu(PluginInfo.PLUGIN_NAME)]
public class Settings : ConfigFile {
  public static Settings Instance { get; } = OptionsPanelHandler.RegisterModOptions<Settings>();

  [Slider(C.LABEL_INV_W, C.INV_MIN_W, C.INV_MAX_W, DefaultValue = C.INV_DEF_W, Step = C.STEP_UNO, Tooltip = $"{C.TOOLTIP_RESTART} Vanilla: 6.")]
  public int invWidth = C.INV_DEF_W;

  [Slider(C.LABEL_INV_H, C.INV_MIN_H, C.INV_MAX_H, DefaultValue = C.INV_DEF_H, Step = C.STEP_UNO, Tooltip = $"{C.TOOLTIP_RESTART} Vanilla: 8.")]
  public int invHeight = C.INV_DEF_H;

  public static int InvWidth => Instance.invWidth;
  public static int InvHeight => Instance.invHeight;
}
