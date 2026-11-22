using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProProductDocuments.Models;

/// <summary>
/// Represents a public product document model
/// </summary>
public partial record PublicProductDocumentModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the document identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the document title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the file extension
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Gets or sets the formatted file size
    /// </summary>
    public string FileSizeFormatted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the document can be displayed in a browser
    /// </summary>
    public bool CanView { get; set; }
}
