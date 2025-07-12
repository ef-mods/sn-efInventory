namespace efInventory;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

using C = Constants;

[Menu(PluginInfo.PLUGIN_NAME)]
public class Settings : ConfigFile {
  public static Settings Instance { get; } = OptionsPanelHandler.RegisterModOptions<Settings>();

  [Toggle(C.LABEL_RESTART)]
  public bool warningDivider;

  [Toggle(C.LABEL_INV_ENABLED)]
  public bool invEnabled = true;

  [Slider(
    C.LABEL_INV_COLS,
    C.INV_COLS_MIN,
    C.INV_COLS_MAX,
    DefaultValue = C.INV_COLS_DEF,
    Step = C.STEP_UNO,
    Tooltip = C.TIP_INV_COLS
  )]
  public int invCols = C.INV_COLS_DEF;

  [Slider(
    C.LABEL_INV_ROWS,
    C.INV_ROWS_MIN,
    C.INV_ROWS_MAX,
    DefaultValue = C.INV_ROWS_DEF,
    Step = C.STEP_UNO,
    Tooltip = C.TIP_INV_ROWS
  )]
  public int invRows = C.INV_ROWS_DEF;

  [Toggle(C.LABEL_BAGS_ENABLED, Tooltip = C.TIP_BAGS_ENABLED)]
  public bool bagsEnabled = true;

  // getters
  public static bool InvEnabled => Instance.invEnabled;
  public static int InvCols => Instance.invCols;
  public static int InvRows => Instance.invRows;
  public static bool BagsEnabled => Instance.bagsEnabled;

}
