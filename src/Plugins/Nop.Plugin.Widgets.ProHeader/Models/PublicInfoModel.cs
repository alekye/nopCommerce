using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProHeader.Models;

/// <summary>
/// Represents an info bar model on the site
/// </summary>
public record PublicInfoModel : BaseNopModel
{
    #region Properties

    public string PictureUrl { get; set; }

    public string Text { get; set; }

    public string TextColor { get; set; }

    public string BackgroundColor { get; set; }

    public int OverlayOpacity { get; set; }

    public int DesktopHeight { get; set; }

    public int MobileHeight { get; set; }

    public string AltText { get; set; }

    #endregion
}
