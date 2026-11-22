using Nop.Core;

namespace Nop.Plugin.Widgets.ProProductDocuments.Domain;

/// <summary>
/// Represents a product document (a file available for download on the product details page)
/// </summary>
public partial class ProductDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the document title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the download identifier
    /// </summary>
    public int DownloadId { get; set; }

    /// <summary>
    /// Gets or sets the original file name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the file content type
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file size (in bytes)
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the document is active
    /// </summary>
    public bool IsActive { get; set; }
}
