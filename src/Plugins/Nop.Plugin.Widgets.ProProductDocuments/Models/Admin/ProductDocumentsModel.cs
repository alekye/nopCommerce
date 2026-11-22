using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.ProProductDocuments.Models.Admin;

/// <summary>
/// Represents a product documents list model
/// </summary>
public partial record ProductDocumentsModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the documents
    /// </summary>
    public IList<ProductDocumentModel> Documents { get; set; } = new List<ProductDocumentModel>();
}
