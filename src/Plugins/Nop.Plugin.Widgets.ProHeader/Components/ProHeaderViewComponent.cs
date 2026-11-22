using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Widgets.ProHeader.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.ProHeader.Components;

/// <summary>
/// Represents the info bar view component
/// </summary>
public class ProHeaderViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ProHeaderViewComponent(IPictureService pictureService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings)
    {
        _pictureService = pictureService;
        _settingService = settingService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a value indicating whether the current page is the store home page
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the current page is the home page; otherwise false
    /// </returns>
    protected virtual async Task<bool> IsHomePageAsync()
    {
        var storeLocationUri = new Uri(_webHelper.GetStoreLocation().TrimEnd('/'));
        var currentPageUri = new Uri(_webHelper.GetThisPageUrl(false).TrimEnd('/'));

        if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            return storeLocationUri.Equals(currentPageUri);

        var currentLanguage = await _workContext.GetWorkingLanguageAsync();

        return Uri.TryCreate(storeLocationUri, currentLanguage.UniqueSeoCode, out var result)
               && result.Equals(currentPageUri);
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        //the info bar is displayed on the home page only
        if (!await IsHomePageAsync())
            return Content("");

        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<ProHeaderSettings>(store.Id);

        var hasText = !string.IsNullOrWhiteSpace(settings.Text);
        if (!settings.Enabled || (!hasText && settings.BackgroundPictureId == 0))
            return Content("");

        var pictureUrl = settings.BackgroundPictureId > 0
            ? await _pictureService.GetPictureUrlAsync(settings.BackgroundPictureId, targetSize: 0, showDefaultPicture: false)
            : string.Empty;

        var model = new PublicInfoModel
        {
            PictureUrl = pictureUrl,
            Text = settings.Text,
            TextColor = settings.TextColor,
            BackgroundColor = settings.BackgroundColor,
            OverlayOpacity = settings.OverlayOpacity,
            DesktopHeight = settings.DesktopHeight,
            MobileHeight = settings.MobileHeight,
            AltText = settings.AltText
        };

        return View("~/Plugins/Widgets.ProHeader/Views/PublicInfo.cshtml", model);
    }

    #endregion
}
