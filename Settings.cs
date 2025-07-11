namespace efInventory;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

using C = Constants;

[Menu(PluginInfo.PLUGIN_NAME)]
public class Settings : ConfigFile {
  public static Settings Instance { get; } = OptionsPanelHandler.RegisterModOptions<Settings>();

  [Slider(C.LABEL_INV_W, C.MIN_W, C.MAX_W, DefaultValue = C.DEF_W, Step = C.STEP_UNO, Tooltip = $"{C.TOOLTIP_RESTART} Vanilla: 6.")]
  public int invWidth = C.DEF_W;

  [Slider(C.LABEL_INV_H, C.MIN_H, C.MAX_H, DefaultValue = C.DEF_H, Step = C.STEP_UNO, Tooltip = $"{C.TOOLTIP_RESTART} Vanilla: 8.")]
  public int invHeight = C.DEF_H;

  public static int InvWidth => Instance.invWidth;
  public static int InvHeight => Instance.invHeight;
}
