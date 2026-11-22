using Nop.Data;
using Nop.Plugin.Widgets.ProProductDocuments.Domain;

namespace Nop.Plugin.Widgets.ProProductDocuments.Services;

/// <summary>
/// Represents a product document service
/// </summary>
public partial class ProductDocumentService : IProductDocumentService
{
    #region Fields

    protected readonly IRepository<ProductDocument> _productDocumentRepository;

    #endregion

    #region Ctor

    public ProductDocumentService(IRepository<ProductDocument> productDocumentRepository)
    {
        _productDocumentRepository = productDocumentRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a product document by identifier
    /// </summary>
    /// <param name="productDocumentId">Product document identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product document
    /// </returns>
    public virtual async Task<ProductDocument> GetProductDocumentByIdAsync(int productDocumentId)
    {
        return await _productDocumentRepository.GetByIdAsync(productDocumentId);
    }

    /// <summary>
    /// Gets all documents of the product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="onlyActive">Whether to load only active documents</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product documents
    /// </returns>
    public virtual async Task<IList<ProductDocument>> GetProductDocumentsByProductIdAsync(int productId, bool onlyActive = false)
    {
        var query = _productDocumentRepository.Table.Where(document => document.ProductId == productId);

        if (onlyActive)
            query = query.Where(document => document.IsActive);

        return await query.OrderBy(document => document.DisplayOrder).ThenBy(document => document.Id).ToListAsync();
    }

    /// <summary>
    /// Gets the next display order for a new product document
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the display order
    /// </returns>
    public virtual async Task<int> GetNextDisplayOrderAsync(int productId)
    {
        var maxDisplayOrder = await _productDocumentRepository.Table
            .Where(document => document.ProductId == productId)
            .Select(document => (int?)document.DisplayOrder)
            .MaxAsync();

        return (maxDisplayOrder ?? 0) + 1;
    }

    /// <summary>
    /// Inserts a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductDocumentAsync(ProductDocument productDocument)
    {
        await _productDocumentRepository.InsertAsync(productDocument);
    }

    /// <summary>
    /// Updates a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductDocumentAsync(ProductDocument productDocument)
    {
        await _productDocumentRepository.UpdateAsync(productDocument);
    }

    /// <summary>
    /// Deletes a product document
    /// </summary>
    /// <param name="productDocument">Product document</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductDocumentAsync(ProductDocument productDocument)
    {
        await _productDocumentRepository.DeleteAsync(productDocument);
    }

    #endregion
}
