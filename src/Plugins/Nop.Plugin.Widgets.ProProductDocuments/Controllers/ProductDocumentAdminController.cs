using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.ProProductDocuments.Domain;
using Nop.Plugin.Widgets.ProProductDocuments.Factories;
using Nop.Plugin.Widgets.ProProductDocuments.Infrastructure;
using Nop.Plugin.Widgets.ProProductDocuments.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.ProProductDocuments.Controllers;

/// <summary>
/// Represents the admin controller to manage product documents
/// </summary>
[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public partial class ProductDocumentAdminController : BasePluginController
{
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IProductDocumentModelFactory _productDocumentModelFactory;
    protected readonly IProductDocumentService _productDocumentService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor

    public ProductDocumentAdminController(IDownloadService downloadService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        IProductDocumentModelFactory productDocumentModelFactory,
        IProductDocumentService productDocumentService,
        IProductService productService)
    {
        _downloadService = downloadService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _productDocumentModelFactory = productDocumentModelFactory;
        _productDocumentService = productDocumentService;
        _productService = productService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get a document title; use the file name when the title is empty
    /// </summary>
    /// <param name="title">Document title</param>
    /// <param name="fileName">File name</param>
    /// <returns>Normalized title</returns>
    protected virtual string NormalizeTitle(string title, string fileName)
    {
        title = string.IsNullOrWhiteSpace(title) ? _fileProvider.GetFileNameWithoutExtension(fileName) : title.Trim();

        return title.Length > 400 ? title[..400] : title;
    }

    /// <summary>
    /// Get a localized error result
    /// </summary>
    /// <param name="resourceKey">Resource key</param>
    /// <param name="args">Message arguments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the JSON result
    /// </returns>
    protected virtual async Task<IActionResult> ErrorJsonAsync(string resourceKey, params object[] args)
    {
        var message = await _localizationService.GetResourceAsync(resourceKey);
        if (args is { Length: > 0 })
            message = string.Format(message, args);

        return Json(new { success = false, message });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the documents list of the product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the partial view result
    /// </returns>
    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> List(int productId)
    {
        var model = await _productDocumentModelFactory.PrepareProductDocumentsModelAsync(productId);

        return PartialView("~/Plugins/Widgets.ProProductDocuments/Views/Admin/_ProductDocumentsList.cshtml", model);
    }

    /// <summary>
    /// Upload a new document
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="title">Document title</param>
    /// <param name="file">Uploaded file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the JSON result
    /// </returns>
    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Upload(int productId, string title, IFormFile file)
    {
        if (await _productService.GetProductByIdAsync(productId) is null)
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.Upload.Failed");

        if (file is null || file.Length == 0)
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.FileRequired");

        var fileName = _fileProvider.GetFileName(file.FileName);
        var fileExtension = _fileProvider.GetFileExtension(fileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(fileExtension) || !ProductDocumentsDefaults.AllowedFileExtensions.Contains(fileExtension))
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.FileTypeNotAllowed");

        if (file.Length > ProductDocumentsDefaults.MaxFileSizeBytes)
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.FileSizeExceeded",
                (ProductDocumentsDefaults.MaxFileSizeBytes / 1024d / 1024d).ToString(CultureInfo.InvariantCulture));

        var fileBinary = await _downloadService.GetDownloadBitsAsync(file);
        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = string.Empty,
            DownloadBinary = fileBinary,
            ContentType = file.ContentType,
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        var productDocument = new ProductDocument
        {
            ProductId = productId,
            Title = NormalizeTitle(title, fileName),
            DownloadId = download.Id,
            FileName = fileName,
            ContentType = file.ContentType,
            FileSize = fileBinary.LongLength,
            DisplayOrder = await _productDocumentService.GetNextDisplayOrderAsync(productId),
            IsActive = true
        };
        await _productDocumentService.InsertProductDocumentAsync(productDocument);

        return Json(new { success = true, id = productDocument.Id });
    }

    /// <summary>
    /// Update an existing document
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <param name="title">Document title</param>
    /// <param name="displayOrder">Display order</param>
    /// <param name="isActive">Whether the document is active</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the JSON result
    /// </returns>
    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Update(int id, string title, int displayOrder, bool isActive)
    {
        var productDocument = await _productDocumentService.GetProductDocumentByIdAsync(id);
        if (productDocument is null)
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.Update.Failed");

        productDocument.Title = NormalizeTitle(title, productDocument.FileName);
        productDocument.DisplayOrder = displayOrder;
        productDocument.IsActive = isActive;

        await _productDocumentService.UpdateProductDocumentAsync(productDocument);

        return Json(new { success = true });
    }

    /// <summary>
    /// Delete a document
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the JSON result
    /// </returns>
    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var productDocument = await _productDocumentService.GetProductDocumentByIdAsync(id);
        if (productDocument is null)
            return await ErrorJsonAsync("Plugins.Widgets.ProProductDocuments.Delete.Failed");

        var download = await _downloadService.GetDownloadByIdAsync(productDocument.DownloadId);

        await _productDocumentService.DeleteProductDocumentAsync(productDocument);

        if (download is not null)
            await _downloadService.DeleteDownloadAsync(download);

        return Json(new { success = true });
    }

    /// <summary>
    /// Download a document (admin preview)
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file result
    /// </returns>
    [HttpGet]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> Download(int id)
    {
        var productDocument = await _productDocumentService.GetProductDocumentByIdAsync(id);
        if (productDocument is null)
            return NotFound();

        var download = await _downloadService.GetDownloadByIdAsync(productDocument.DownloadId);
        if (download is null)
            return NotFound();

        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        if (download.DownloadBinary is null)
            return NotFound();

        var extension = ProductDocumentFileHelper.GetFileExtension(download.Extension, productDocument.FileName);
        var fileName = !string.IsNullOrWhiteSpace(download.Filename)
            ? download.Filename
            : _fileProvider.GetFileNameWithoutExtension(productDocument.FileName);
        var contentType = ProductDocumentFileHelper.GetContentType(extension, download.ContentType);

        return new FileContentResult(download.DownloadBinary, contentType)
        {
            FileDownloadName = fileName + extension
        };
    }

    #endregion
}
