using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.ProProductDocuments.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.ProProductDocuments.Components;

/// <summary>
/// Represents the view component to display product documents on the product details page
/// </summary>
public partial class ProductDocumentsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IProductDocumentModelFactory _productDocumentModelFactory;

    #endregion

    #region Ctor

    public ProductDocumentsViewComponent(IProductDocumentModelFactory productDocumentModelFactory)
    {
        _productDocumentModelFactory = productDocumentModelFactory;
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
        if (additionalData is not ProductDetailsModel productModel)
            return Content(string.Empty);

        var model = await _productDocumentModelFactory.PreparePublicProductDocumentsModelAsync(productModel.Id);
        if (!model.Documents.Any())
            return Content(string.Empty);

        return View("~/Plugins/Widgets.ProProductDocuments/Views/ProductDocuments.cshtml", model);
    }

    #endregion
}
