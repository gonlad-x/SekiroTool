using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SekiroTool.Utilities;

namespace SekiroTool.Views.Windows;

public partial class OverlayCustomizationWindow : Window
{
    private const string DefaultFontFamily = "Segoe UI";
    private const double DefaultLabelFontSize = 14;
    private const double DefaultValueFontSize = 14;
    private const double DefaultHitCountFontSize = 16;
    private const double DefaultStaggerFontSize = 14;

    private const string DefaultLabelColor = "#CDD6F4";
    private const string DefaultValueColor = "#04A5E5";
    private const string DefaultHitCountColor = "#04A5E5";
    private const string DefaultStaggerColor = "#CDD6F4";
    private const string DefaultBarColor = "#F2CDCD";
    private const double DefaultBackgroundOpacity = 0.0;
    private const double DefaultRowHeight = 24;

    private const string DefaultHitsLabelText = "Hits to stagger:";
    private const string DefaultHpLabelText = "HP:";
    private const string DefaultPostureLabelText = "Posture:";
    private const string DefaultPoiseTimerLabelText = "Poise timer:";
    private const string DefaultActLabelText = "Act:";
    private const string DefaultKengekiLabelText = "Kengeki:";

    public OverlayCustomizationWindow()
    {
        InitializeComponent();

        var families = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(s => s).ToList();
        LabelFontFamilyComboBox.ItemsSource = families;
        ValueFontFamilyComboBox.ItemsSource = families;
        HitCountFontFamilyComboBox.ItemsSource = families;
        StaggerFontFamilyComboBox.ItemsSource = families;

        SetupColorField(LabelColorSwatch, LabelColorTextBox);
        SetupColorField(ValueColorSwatch, ValueColorTextBox);
        SetupColorField(HitCountColorSwatch, HitCountColorTextBox);
        SetupColorField(StaggerColorSwatch, StaggerColorTextBox);
        SetupColorField(BarColorSwatch, BarColorTextBox);

        LoadSettings();
    }

    private void LoadSettings()
    {
        var s = SettingsManager.Default;

        LabelFontFamilyComboBox.Text = OrDefault(s.BrowserOverlayLabelFontFamily);
        LabelFontSizeUpDown.Value = s.BrowserOverlayLabelFontSize > 0 ? s.BrowserOverlayLabelFontSize : DefaultLabelFontSize;
        SetColorField(LabelColorSwatch, LabelColorTextBox, s.BrowserOverlayLabelColor, DefaultLabelColor);

        ValueFontFamilyComboBox.Text = OrDefault(s.BrowserOverlayValueFontFamily);
        ValueFontSizeUpDown.Value = s.BrowserOverlayValueFontSize > 0 ? s.BrowserOverlayValueFontSize : DefaultValueFontSize;
        SetColorField(ValueColorSwatch, ValueColorTextBox, s.BrowserOverlayValueColor, DefaultValueColor);

        HitCountFontFamilyComboBox.Text = OrDefault(s.BrowserOverlayHitCountFontFamily);
        HitCountFontSizeUpDown.Value = s.BrowserOverlayHitCountFontSize > 0 ? s.BrowserOverlayHitCountFontSize : DefaultHitCountFontSize;
        SetColorField(HitCountColorSwatch, HitCountColorTextBox, s.BrowserOverlayHitCountColor, DefaultHitCountColor);

        StaggerFontFamilyComboBox.Text = OrDefault(s.BrowserOverlayStaggerThresholdFontFamily);
        StaggerFontSizeUpDown.Value = s.BrowserOverlayStaggerThresholdFontSize > 0 ? s.BrowserOverlayStaggerThresholdFontSize : DefaultStaggerFontSize;
        SetColorField(StaggerColorSwatch, StaggerColorTextBox, s.BrowserOverlayStaggerThresholdColor, DefaultStaggerColor);

        SetColorField(BarColorSwatch, BarColorTextBox, s.BrowserOverlayBarColor, DefaultBarColor);
        RowHeightUpDown.Value = s.BrowserOverlayRowHeight > 0 ? s.BrowserOverlayRowHeight : DefaultRowHeight;
        BackgroundOpacityUpDown.Value = s.BrowserOverlayBackgroundOpacity;

        HitsLabelTextBox.Text = s.BrowserOverlayHitsLabelText ?? DefaultHitsLabelText;
        HpLabelTextBox.Text = s.BrowserOverlayHpLabelText ?? DefaultHpLabelText;
        PostureLabelTextBox.Text = s.BrowserOverlayPostureLabelText ?? DefaultPostureLabelText;
        PoiseTimerLabelTextBox.Text = s.BrowserOverlayPoiseTimerLabelText ?? DefaultPoiseTimerLabelText;
        ActLabelTextBox.Text = s.BrowserOverlayActLabelText ?? DefaultActLabelText;
        KengekiLabelTextBox.Text = s.BrowserOverlayKengekiLabelText ?? DefaultKengekiLabelText;

        ShowLabelsCheckBox.IsChecked = s.BrowserOverlayShowLabels;
        ShowHitsRowCheckBox.IsChecked = s.BrowserOverlayShowHitsRow;
        ShowPoiseBarCheckBox.IsChecked = s.BrowserOverlayShowPoiseBar;
        ShowHpCheckBox.IsChecked = s.BrowserOverlayShowHp;
        ShowPostureCheckBox.IsChecked = s.BrowserOverlayShowPosture;
        ShowPoiseTimerCheckBox.IsChecked = s.BrowserOverlayShowPoiseTimer;
        ShowActKengekiCheckBox.IsChecked = s.BrowserOverlayShowActKengeki;
    }

    private static string OrDefault(string value) =>
        string.IsNullOrWhiteSpace(value) ? DefaultFontFamily : value;

    private void SetupColorField(Border swatch, TextBox hexBox)
    {
        hexBox.TextChanged += (_, _) =>
        {
            if (TryParseColorText(hexBox.Text, out var color))
                swatch.Background = new SolidColorBrush(color);
        };
    }

    private static void SetColorField(Border swatch, TextBox hexBox, string hex, string fallbackHex)
    {
        Color color = ParseColor(hex, fallbackHex);
        swatch.Background = new SolidColorBrush(color);
        hexBox.Text = ColorToHex(color);
    }

    private static Color ParseColor(string hex, string fallbackHex)
    {
        if (TryParseColorText(hex, out var color)) return color;
        return TryParseColorText(fallbackHex, out var fallback) ? fallback : Colors.White;
    }

    private static bool TryParseColorText(string text, out Color color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        text = text.Trim();

        if (text.StartsWith("rgb", System.StringComparison.OrdinalIgnoreCase))
        {
            int start = text.IndexOf('(');
            int end = text.IndexOf(')');
            if (start >= 0 && end > start) text = text.Substring(start + 1, end - start - 1);
        }

        if (text.Contains(','))
        {
            var parts = text.Split(',').Select(p => p.Trim()).ToArray();
            if ((parts.Length == 3 || parts.Length == 4) && parts.All(p => byte.TryParse(p, out _)))
            {
                byte r = byte.Parse(parts[0]);
                byte g = byte.Parse(parts[1]);
                byte b = byte.Parse(parts[2]);
                byte a = parts.Length == 4 ? byte.Parse(parts[3]) : (byte)255;
                color = Color.FromArgb(a, r, g, b);
                return true;
            }

            return false;
        }

        string hex = text.StartsWith("#") ? text : "#" + text;
        try
        {
            color = (Color)ColorConverter.ConvertFromString(hex);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var s = SettingsManager.Default;

        s.BrowserOverlayLabelFontFamily = OrDefault(LabelFontFamilyComboBox.Text);
        s.BrowserOverlayLabelFontSize = LabelFontSizeUpDown.Value ?? DefaultLabelFontSize;
        s.BrowserOverlayLabelColor = ColorToHex(ParseColor(LabelColorTextBox.Text, DefaultLabelColor));

        s.BrowserOverlayValueFontFamily = OrDefault(ValueFontFamilyComboBox.Text);
        s.BrowserOverlayValueFontSize = ValueFontSizeUpDown.Value ?? DefaultValueFontSize;
        s.BrowserOverlayValueColor = ColorToHex(ParseColor(ValueColorTextBox.Text, DefaultValueColor));

        s.BrowserOverlayHitCountFontFamily = OrDefault(HitCountFontFamilyComboBox.Text);
        s.BrowserOverlayHitCountFontSize = HitCountFontSizeUpDown.Value ?? DefaultHitCountFontSize;
        s.BrowserOverlayHitCountColor = ColorToHex(ParseColor(HitCountColorTextBox.Text, DefaultHitCountColor));

        s.BrowserOverlayStaggerThresholdFontFamily = OrDefault(StaggerFontFamilyComboBox.Text);
        s.BrowserOverlayStaggerThresholdFontSize = StaggerFontSizeUpDown.Value ?? DefaultStaggerFontSize;
        s.BrowserOverlayStaggerThresholdColor = ColorToHex(ParseColor(StaggerColorTextBox.Text, DefaultStaggerColor));

        s.BrowserOverlayBarColor = ColorToHex(ParseColor(BarColorTextBox.Text, DefaultBarColor));
        s.BrowserOverlayRowHeight = RowHeightUpDown.Value ?? DefaultRowHeight;
        s.BrowserOverlayBackgroundOpacity = BackgroundOpacityUpDown.Value ?? DefaultBackgroundOpacity;

        s.BrowserOverlayHitsLabelText = HitsLabelTextBox.Text;
        s.BrowserOverlayHpLabelText = HpLabelTextBox.Text;
        s.BrowserOverlayPostureLabelText = PostureLabelTextBox.Text;
        s.BrowserOverlayPoiseTimerLabelText = PoiseTimerLabelTextBox.Text;
        s.BrowserOverlayActLabelText = ActLabelTextBox.Text;
        s.BrowserOverlayKengekiLabelText = KengekiLabelTextBox.Text;

        s.BrowserOverlayShowLabels = ShowLabelsCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowHitsRow = ShowHitsRowCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowPoiseBar = ShowPoiseBarCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowHp = ShowHpCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowPosture = ShowPostureCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowPoiseTimer = ShowPoiseTimerCheckBox.IsChecked ?? true;
        s.BrowserOverlayShowActKengeki = ShowActKengekiCheckBox.IsChecked ?? true;
        s.Save();

        BrowserOverlayExporter.WriteConfig();

        Close();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        LabelFontFamilyComboBox.Text = DefaultFontFamily;
        LabelFontSizeUpDown.Value = DefaultLabelFontSize;
        SetColorField(LabelColorSwatch, LabelColorTextBox, DefaultLabelColor, DefaultLabelColor);

        ValueFontFamilyComboBox.Text = DefaultFontFamily;
        ValueFontSizeUpDown.Value = DefaultValueFontSize;
        SetColorField(ValueColorSwatch, ValueColorTextBox, DefaultValueColor, DefaultValueColor);

        HitCountFontFamilyComboBox.Text = DefaultFontFamily;
        HitCountFontSizeUpDown.Value = DefaultHitCountFontSize;
        SetColorField(HitCountColorSwatch, HitCountColorTextBox, DefaultHitCountColor, DefaultHitCountColor);

        StaggerFontFamilyComboBox.Text = DefaultFontFamily;
        StaggerFontSizeUpDown.Value = DefaultStaggerFontSize;
        SetColorField(StaggerColorSwatch, StaggerColorTextBox, DefaultStaggerColor, DefaultStaggerColor);

        SetColorField(BarColorSwatch, BarColorTextBox, DefaultBarColor, DefaultBarColor);
        RowHeightUpDown.Value = DefaultRowHeight;
        BackgroundOpacityUpDown.Value = DefaultBackgroundOpacity;

        HitsLabelTextBox.Text = DefaultHitsLabelText;
        HpLabelTextBox.Text = DefaultHpLabelText;
        PostureLabelTextBox.Text = DefaultPostureLabelText;
        PoiseTimerLabelTextBox.Text = DefaultPoiseTimerLabelText;
        ActLabelTextBox.Text = DefaultActLabelText;
        KengekiLabelTextBox.Text = DefaultKengekiLabelText;

        ShowLabelsCheckBox.IsChecked = true;
        ShowHitsRowCheckBox.IsChecked = true;
        ShowPoiseBarCheckBox.IsChecked = true;
        ShowHpCheckBox.IsChecked = true;
        ShowPostureCheckBox.IsChecked = true;
        ShowPoiseTimerCheckBox.IsChecked = true;
        ShowActKengekiCheckBox.IsChecked = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
}
