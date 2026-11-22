using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.ProProductDocuments.Factories;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.ProProductDocuments.Components;

/// <summary>
/// Represents the view component to manage product documents on the product edit page
/// </summary>
public partial class AdminProductDocumentsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IProductDocumentModelFactory _productDocumentModelFactory;
    protected readonly IPermissionService _permissionService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AdminProductDocumentsViewComponent(IProductDocumentModelFactory productDocumentModelFactory,
        IPermissionService permissionService,
        IWorkContext workContext)
    {
        _productDocumentModelFactory = productDocumentModelFactory;
        _permissionService = permissionService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (additionalData is not ProductModel productModel)
            return Content(string.Empty);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW, customer))
            return Content(string.Empty);

        var model = await _productDocumentModelFactory.PrepareProductDocumentsModelAsync(productModel.Id);

        return View("~/Plugins/Widgets.ProProductDocuments/Views/Admin/ProductDocuments.cshtml", model);
    }

    #endregion
}
