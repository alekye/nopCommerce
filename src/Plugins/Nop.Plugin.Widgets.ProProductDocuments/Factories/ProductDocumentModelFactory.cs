using System.Globalization;
using Nop.Plugin.Widgets.ProProductDocuments.Infrastructure;
using Nop.Plugin.Widgets.ProProductDocuments.Models;
using Nop.Plugin.Widgets.ProProductDocuments.Models.Admin;
using Nop.Plugin.Widgets.ProProductDocuments.Services;

namespace Nop.Plugin.Widgets.ProProductDocuments.Factories;

/// <summary>
/// Represents a product document model factory interface
/// </summary>
public partial interface IProductDocumentModelFactory
{
    /// <summary>
    /// Prepare the product documents model
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product documents model
    /// </returns>
    Task<ProductDocumentsModel> PrepareProductDocumentsModelAsync(int productId);

    /// <summary>
    /// Prepare the public product documents model
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the public product documents model
    /// </returns>
    Task<PublicProductDocumentsModel> PreparePublicProductDocumentsModelAsync(int productId);
}

/// <summary>
/// Represents a product document model factory
/// </summary>
public partial class ProductDocumentModelFactory : IProductDocumentModelFactory
{
    #region Fields

    protected readonly IProductDocumentService _productDocumentService;

    #endregion

    #region Ctor

    public ProductDocumentModelFactory(IProductDocumentService productDocumentService)
    {
        _productDocumentService = productDocumentService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get a lower-cased file extension (with the leading dot)
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <returns>File extension</returns>
    protected virtual string GetFileExtension(string fileName)
    {
        return ProductDocumentFileHelper.GetFileExtension(null, fileName);
    }

    /// <summary>
    /// Format file size
    /// </summary>
    /// <param name="fileSize">File size in bytes</param>
    /// <returns>Formatted file size</returns>
    protected virtual string FormatFileSize(long fileSize)
    {
        if (fileSize <= 0)
            return string.Empty;

        string[] units = ["B", "KB", "MB", "GB"];
        double size = fileSize;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size.ToString("0.#", CultureInfo.InvariantCulture)} {units[unitIndex]}";
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the product documents model
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product documents model
    /// </returns>
    public virtual async Task<ProductDocumentsModel> PrepareProductDocumentsModelAsync(int productId)
    {
        var model = new ProductDocumentsModel { ProductId = productId };

        if (productId == 0)
            return model;

        foreach (var document in await _productDocumentService.GetProductDocumentsByProductIdAsync(productId))
        {
            model.Documents.Add(new ProductDocumentModel
            {
                Id = document.Id,
                ProductId = document.ProductId,
                Title = document.Title,
                FileName = document.FileName,
                FileExtension = GetFileExtension(document.FileName),
                FileSizeFormatted = FormatFileSize(document.FileSize),
                DisplayOrder = document.DisplayOrder,
                IsActive = document.IsActive
            });
        }

        return model;
    }

    /// <summary>
    /// Prepare the public product documents model
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the public product documents model
    /// </returns>
    public virtual async Task<PublicProductDocumentsModel> PreparePublicProductDocumentsModelAsync(int productId)
    {
        var model = new PublicProductDocumentsModel();

        if (productId == 0)
            return model;

        foreach (var document in await _productDocumentService.GetProductDocumentsByProductIdAsync(productId, onlyActive: true))
        {
            var extension = GetFileExtension(document.FileName);

            model.Documents.Add(new PublicProductDocumentModel
            {
                Id = document.Id,
                Title = document.Title,
                FileExtension = extension.TrimStart('.'),
                FileSizeFormatted = FormatFileSize(document.FileSize),
                CanView = ProductDocumentFileHelper.CanDisplayInline(extension)
            });
        }

        return model;
    }

    #endregion
}
