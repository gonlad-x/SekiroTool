using System.Globalization;
using System.IO;
using System.Reflection;

namespace SekiroTool.Utilities;

public static class BrowserOverlayExporter
{
    private const string ResourceName = "SekiroTool.Assets.BrowserOverlay.html";

    public static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SekiroTool", "BrowserOverlay");

    private static readonly string HtmlPath = Path.Combine(FolderPath, "overlay.html");
    private static readonly string DataPath = Path.Combine(FolderPath, "overlay_data.json");
    private static readonly string ConfigPath = Path.Combine(FolderPath, "overlay_config.json");

    private static bool _htmlExported;

    public static void EnsureHtmlExported()
    {
        if (_htmlExported && File.Exists(HtmlPath)) return;

        try
        {
            Directory.CreateDirectory(FolderPath);

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(ResourceName);
            if (stream == null) return;

            using var fileStream = File.Create(HtmlPath);
            stream.CopyTo(fileStream);

            _htmlExported = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error exporting browser overlay html: {ex.Message}");
        }
    }

    public static void Write(bool hasValidPoise, float hitCount, float staggerThreshold,
        float currentPoise, float maxPoise, float poiseTimer, double healthPercentage, double posturePercentage,
        int lastAct, int lastKengekiAct)
    {
        string json = "{" +
                      $"\"active\":true," +
                      $"\"hasValidPoise\":{Bool(hasValidPoise)}," +
                      $"\"hitCount\":{Num(hitCount)}," +
                      $"\"staggerThreshold\":{Num(staggerThreshold)}," +
                      $"\"currentPoise\":{Num(currentPoise)}," +
                      $"\"maxPoise\":{Num(maxPoise)}," +
                      $"\"poiseTimer\":{Num(poiseTimer)}," +
                      $"\"healthPercentage\":{Num(healthPercentage)}," +
                      $"\"posturePercentage\":{Num(posturePercentage)}," +
                      $"\"lastAct\":{lastAct}," +
                      $"\"lastKengekiAct\":{lastKengekiAct}" +
                      "}";

        WriteJson(DataPath, json);
    }

    public static void Clear() => WriteJson(DataPath, "{\"active\":false}");

    public static void WriteConfig()
    {
        var s = SettingsManager.Default;

        string json = "{" +
                      $"\"labelFontFamily\":{Str(s.BrowserOverlayLabelFontFamily)}," +
                      $"\"labelFontSize\":{Num(s.BrowserOverlayLabelFontSize)}," +
                      $"\"labelColor\":{Str(s.BrowserOverlayLabelColor)}," +
                      $"\"valueFontFamily\":{Str(s.BrowserOverlayValueFontFamily)}," +
                      $"\"valueFontSize\":{Num(s.BrowserOverlayValueFontSize)}," +
                      $"\"valueColor\":{Str(s.BrowserOverlayValueColor)}," +
                      $"\"hitCountFontFamily\":{Str(s.BrowserOverlayHitCountFontFamily)}," +
                      $"\"hitCountFontSize\":{Num(s.BrowserOverlayHitCountFontSize)}," +
                      $"\"hitCountColor\":{Str(s.BrowserOverlayHitCountColor)}," +
                      $"\"staggerThresholdFontFamily\":{Str(s.BrowserOverlayStaggerThresholdFontFamily)}," +
                      $"\"staggerThresholdFontSize\":{Num(s.BrowserOverlayStaggerThresholdFontSize)}," +
                      $"\"staggerThresholdColor\":{Str(s.BrowserOverlayStaggerThresholdColor)}," +
                      $"\"barColor\":{Str(s.BrowserOverlayBarColor)}," +
                      $"\"backgroundOpacity\":{Num(s.BrowserOverlayBackgroundOpacity)}," +
                      $"\"rowHeight\":{Num(s.BrowserOverlayRowHeight)}," +
                      $"\"hitsLabelText\":{Str(s.BrowserOverlayHitsLabelText)}," +
                      $"\"hpLabelText\":{Str(s.BrowserOverlayHpLabelText)}," +
                      $"\"postureLabelText\":{Str(s.BrowserOverlayPostureLabelText)}," +
                      $"\"poiseTimerLabelText\":{Str(s.BrowserOverlayPoiseTimerLabelText)}," +
                      $"\"actLabelText\":{Str(s.BrowserOverlayActLabelText)}," +
                      $"\"kengekiLabelText\":{Str(s.BrowserOverlayKengekiLabelText)}," +
                      $"\"showLabels\":{Bool(s.BrowserOverlayShowLabels)}," +
                      $"\"showHitsRow\":{Bool(s.BrowserOverlayShowHitsRow)}," +
                      $"\"showPoiseBar\":{Bool(s.BrowserOverlayShowPoiseBar)}," +
                      $"\"showHp\":{Bool(s.BrowserOverlayShowHp)}," +
                      $"\"showPosture\":{Bool(s.BrowserOverlayShowPosture)}," +
                      $"\"showPoiseTimer\":{Bool(s.BrowserOverlayShowPoiseTimer)}," +
                      $"\"showActKengeki\":{Bool(s.BrowserOverlayShowActKengeki)}" +
                      "}";

        WriteJson(ConfigPath, json);
    }

    private static void WriteJson(string path, string json)
    {
        try
        {
            Directory.CreateDirectory(FolderPath);

            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(path)) File.Delete(path);
            File.Move(tempPath, path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error writing browser overlay data: {ex.Message}");
        }
    }

    private static string Bool(bool b) => b ? "true" : "false";

    private static string Num(double d) =>
        Math.Round(d, 2).ToString(CultureInfo.InvariantCulture);

    private static string Str(string s) =>
        "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
}
