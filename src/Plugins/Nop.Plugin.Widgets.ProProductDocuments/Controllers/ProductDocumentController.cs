using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Media;
using Nop.Plugin.Widgets.ProProductDocuments.Infrastructure;
using Nop.Plugin.Widgets.ProProductDocuments.Services;
using Nop.Services.Media;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.ProProductDocuments.Controllers;

/// <summary>
/// Represents the public controller to download product documents
/// </summary>
public partial class ProductDocumentController : BasePublicController
{
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly IProductDocumentService _productDocumentService;

    #endregion

    #region Ctor

    public ProductDocumentController(IDownloadService downloadService,
        IProductDocumentService productDocumentService)
    {
        _downloadService = downloadService;
        _productDocumentService = productDocumentService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Download or view a product document
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <param name="download">Whether to force downloading the file instead of displaying it in a browser</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file result
    /// </returns>
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> Download(int id, bool download = false)
    {
        var productDocument = await _productDocumentService.GetProductDocumentByIdAsync(id);
        if (productDocument is null || !productDocument.IsActive)
            return InvokeHttp404();

        var downloadRecord = await _downloadService.GetDownloadByIdAsync(productDocument.DownloadId);
        if (downloadRecord is null)
            return InvokeHttp404();

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (downloadRecord.UseDownloadUrl)
            return new RedirectResult(downloadRecord.DownloadUrl);

        if (downloadRecord.DownloadBinary is null)
            return InvokeHttp404();

        var extension = ProductDocumentFileHelper.GetFileExtension(downloadRecord.Extension, productDocument.FileName);
        var fileName = !string.IsNullOrWhiteSpace(downloadRecord.Filename)
            ? downloadRecord.Filename
            : Path.GetFileNameWithoutExtension(productDocument.FileName);
        var contentType = ProductDocumentFileHelper.GetContentType(extension, downloadRecord.ContentType);

        //display the file inline (when possible) unless the download is explicitly requested
        var inline = !download && ProductDocumentFileHelper.CanDisplayInline(extension);
        ProductDocumentFileHelper.SetContentDisposition(Response, inline, fileName + extension);

        return File(downloadRecord.DownloadBinary, contentType, enableRangeProcessing: true);
    }

    #endregion
}
