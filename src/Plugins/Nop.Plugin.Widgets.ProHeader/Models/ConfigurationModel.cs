using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.ProHeader.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.Enabled")]
    public bool Enabled { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Widgets.ProHeader.BackgroundPictureId")]
    public int BackgroundPictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.BackgroundColor")]
    public string BackgroundColor { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.Text")]
    public string Text { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.TextColor")]
    public string TextColor { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.OverlayOpacity")]
    public int OverlayOpacity { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.DesktopHeight")]
    public int DesktopHeight { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.MobileHeight")]
    public int MobileHeight { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.ProHeader.AltText")]
    public string AltText { get; set; }

    #endregion
}
