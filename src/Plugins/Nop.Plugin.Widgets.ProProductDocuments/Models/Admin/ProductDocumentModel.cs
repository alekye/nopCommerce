using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.ProProductDocuments.Models.Admin;

/// <summary>
/// Represents a product document model
/// </summary>
public partial record ProductDocumentModel : BaseNopEntityModel
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the document title
    /// </summary>
    [NopResourceDisplayName("Plugins.Widgets.ProProductDocuments.Fields.Title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the original file name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the file extension
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Gets or sets the formatted file size
    /// </summary>
    public string FileSizeFormatted { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    [NopResourceDisplayName("Plugins.Widgets.ProProductDocuments.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the document is active
    /// </summary>
    [NopResourceDisplayName("Plugins.Widgets.ProProductDocuments.Fields.IsActive")]
    public bool IsActive { get; set; }
}
