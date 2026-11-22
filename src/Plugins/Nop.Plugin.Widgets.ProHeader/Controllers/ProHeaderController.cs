using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.ProHeader.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.ProHeader.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ProHeaderController : BasePluginController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ProHeaderController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Configure()
    {
        //load settings for the active store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<ProHeaderSettings>(storeScope);

        var model = new ConfigurationModel
        {
            Enabled = settings.Enabled,
            BackgroundPictureId = settings.BackgroundPictureId,
            BackgroundColor = settings.BackgroundColor,
            Text = settings.Text,
            TextColor = settings.TextColor,
            OverlayOpacity = settings.OverlayOpacity,
            DesktopHeight = settings.DesktopHeight,
            MobileHeight = settings.MobileHeight,
            AltText = settings.AltText,
            ActiveStoreScopeConfiguration = storeScope
        };

        return View("~/Plugins/Widgets.ProHeader/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<ProHeaderSettings>(storeScope);

        settings.Enabled = model.Enabled;
        settings.BackgroundPictureId = model.BackgroundPictureId;
        settings.BackgroundColor = model.BackgroundColor;
        settings.Text = model.Text;
        settings.TextColor = model.TextColor;
        settings.OverlayOpacity = model.OverlayOpacity;
        settings.DesktopHeight = model.DesktopHeight;
        settings.MobileHeight = model.MobileHeight;
        settings.AltText = model.AltText;

        await _settingService.SaveSettingAsync(settings, storeScope);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}
