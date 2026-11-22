using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.ProHeader;

/// <summary>
/// Represents plugin settings
/// </summary>
public class ProHeaderSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the info bar is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the background picture identifier
    /// </summary>
    public int BackgroundPictureId { get; set; }

    /// <summary>
    /// Gets or sets the background color used before the picture is loaded
    /// </summary>
    public string BackgroundColor { get; set; } = "#f2f2f2";

    /// <summary>
    /// Gets or sets the bar content (limited HTML is allowed)
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text color
    /// </summary>
    public string TextColor { get; set; } = "#2156b0";

    /// <summary>
    /// Gets or sets the overlay opacity (0-80) used to improve text contrast
    /// </summary>
    public int OverlayOpacity { get; set; }

    /// <summary>
    /// Gets or sets the desktop bar height in pixels; 0 means the height is derived from the picture ratio
    /// </summary>
    public int DesktopHeight { get; set; } = 220;

    /// <summary>
    /// Gets or sets the mobile bar height in pixels
    /// </summary>
    public int MobileHeight { get; set; } = 120;

    /// <summary>
    /// Gets or sets the background picture alternate text
    /// </summary>
    public string AltText { get; set; } = string.Empty;
}
