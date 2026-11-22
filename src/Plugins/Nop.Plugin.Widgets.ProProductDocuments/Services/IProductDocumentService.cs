using Nop.Plugin.Widgets.ProProductDocuments.Domain;

namespace Nop.Plugin.Widgets.ProProductDocuments.Services;

/// <summary>
/// Represents a product document service interface
/// </summary>
public partial interface IProductDocumentService
{
    /// <summary>
    /// Gets a product document by identifier
    /// </summary>
    /// <param name="productDocumentId">Product document identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product document
    /// </returns>
    Task<ProductDocument> GetProductDocumentByIdAsync(int productDocumentId);

    /// <summary>
    /// Gets all documents of the product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="onlyActive">Whether to load only active documents</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product documents
    /// </returns>
    Task<IList<ProductDocument>> GetProductDocumentsByProductIdAsync(int productId, bool onlyActive = false);

    /// <summary>
    /// Gets the next display order for a new product document
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the display order
    /// </returns>
    Task<int> GetNextDisplayOrderAsync(int productId);

    /// <summary>
    /// Inserts a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductDocumentAsync(ProductDocument productDocument);

    /// <summary>
    /// Updates a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductDocumentAsync(ProductDocument productDocument);

    /// <summary>
    /// Deletes a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductDocumentAsync(ProductDocument productDocument);
}
