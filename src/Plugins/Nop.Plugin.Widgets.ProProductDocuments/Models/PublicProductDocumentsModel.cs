using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProProductDocuments.Models;

/// <summary>
/// Represents a public product documents list model
/// </summary>
public partial record PublicProductDocumentsModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the documents
    /// </summary>
    public IList<PublicProductDocumentModel> Documents { get; set; } = new List<PublicProductDocumentModel>();
}
