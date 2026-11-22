using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.Swiper.Domain;
using Nop.Plugin.Widgets.Swiper.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Swiper.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class WidgetSwiperController : BasePluginController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public WidgetSwiperController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPictureService pictureService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _pictureService = pictureService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task<List<Slide>> GetSlidesForStoreAsync(int storeId)
    {
        var key = $"{nameof(SwiperSettings)}.{nameof(SwiperSettings.Slides)}";

        //load settings for a chosen store scope
        var slidesSetting = await _settingService.GetSettingByKeyAsync(key, string.Empty, storeId: storeId, loadSharedValueIfNotFound: false);

        if (string.IsNullOrEmpty(slidesSetting))
            return new List<Slide>();

        return JsonConvert.DeserializeObject<List<Slide>>(slidesSetting) ?? new List<Slide>();
    }

    protected virtual async Task SaveSlidesForStoreAsync(List<Slide> slides, int storeId)
    {
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeId);

        if (slides.Count == 0)
        {
            await _settingService.DeleteSettingAsync(sliderSettings, setting => setting.Slides, storeId);
            return;
        }

        sliderSettings.Slides = JsonConvert.SerializeObject(slides);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Slides, true, storeId);
    }

    protected virtual async Task<ConfigurationModel> PrepareConfigurationModelAsync()
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);

        var model = new ConfigurationModel
        {
            ShowNavigation = sliderSettings.ShowNavigation,
            ShowPagination = sliderSettings.ShowPagination,
            Autoplay = sliderSettings.Autoplay,
            AutoplayDelay = sliderSettings.AutoplayDelay,
            LazyLoading = sliderSettings.LazyLoading,
            ActiveStoreScopeConfiguration = storeScope
        };

        if (storeScope > 0)
        {
            model.ShowNavigation_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.ShowNavigation, storeScope);
            model.Autoplay_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.Autoplay, storeScope);
            model.AutoplayDelay_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.AutoplayDelay, storeScope);
            model.LazyLoading_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.LazyLoading, storeScope);
        }

        return model;
    }

    protected virtual IActionResult ConfigureView(ConfigurationModel model)
    {
        return View("~/Plugins/Widgets.Swiper/Views/Configure.cshtml", model);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure()
    {
        return ConfigureView(await PrepareConfigurationModelAsync());
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);

        sliderSettings.ShowNavigation = model.ShowNavigation;
        sliderSettings.ShowPagination = model.ShowPagination;
        sliderSettings.Autoplay = model.Autoplay;
        sliderSettings.AutoplayDelay = model.AutoplayDelay;
        sliderSettings.LazyLoading = model.LazyLoading;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.ShowNavigation, model.ShowNavigation_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.ShowPagination, model.ShowPagination_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Autoplay, model.Autoplay_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.AutoplayDelay, model.AutoplayDelay_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.LazyLoading, model.LazyLoading_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [IgnoreAntiforgeryToken]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("add-slide")]
    public virtual async Task<IActionResult> SlideAdd(SlidePictureModel model)
    {
        //a slide must always reference an existing picture, otherwise the grid cannot display it
        if (await _pictureService.GetPictureByIdAsync(model.PictureId) is null)
            ModelState.AddModelError(nameof(model.PictureId), await _localizationService.GetResourceAsync("Plugins.Widgets.Swiper.Picture.Required"));

        if (!ModelState.IsValid)
        {
            //display validation errors and keep the entered values
            var configurationModel = await PrepareConfigurationModelAsync();
            configurationModel.AddSlideModel = model;

            return ConfigureView(configurationModel);
        }

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var slides = await GetSlidesForStoreAsync(storeScope);

        slides.Add(new()
        {
            PictureId = model.PictureId,
            AltText = model.AltText,
            TitleText = model.TitleText,
            LinkUrl = model.LinkUrl
        });

        await SaveSlidesForStoreAsync(slides, storeScope);

        return RedirectToAction(nameof(Configure));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> SlideList(SlidesSearchModel slidesSearchModel)
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var slides = await GetSlidesForStoreAsync(storeScope);

        //a slide may reference a picture that has been deleted in the meantime;
        //skip such records instead of breaking the whole grid
        var slideModels = new List<PublicSlideModel>();
        foreach (var slide in slides.Where(slide => slide.PictureId != 0))
        {
            var picture = await _pictureService.GetPictureByIdAsync(slide.PictureId);
            if (picture is null)
                continue;

            slideModels.Add(new PublicSlideModel
            {
                PictureId = slide.PictureId,
                PictureUrl = (await _pictureService.GetPictureUrlAsync(picture, 200)).Url,
                TitleText = slide.TitleText,
                AltText = slide.AltText,
                LinkUrl = slide.LinkUrl
            });
        }

        var pagedSlideModels = slideModels.ToPagedList(slidesSearchModel);
        var model = new SlideListModel().PrepareToGrid(slidesSearchModel, pagedSlideModels, () => pagedSlideModels);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> SlideDelete(int pictureId)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

        var slides = await GetSlidesForStoreAsync(storeScope);

        if (slides.RemoveAll(slide => slide.PictureId == pictureId) == 0)
            return new NullJsonResult();

        //the picture may have been deleted already, but the slide should be removed anyway
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
        if (picture is not null)
            await _pictureService.DeletePictureAsync(picture);

        await SaveSlidesForStoreAsync(slides, storeScope);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> SlideEdit(SlidePictureModel model)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

        var slides = await GetSlidesForStoreAsync(storeScope);
        if (slides.Count == 0)
            return Content("No slides");

        //try to get a picture with the specified id
        var slide = slides.FirstOrDefault(s => s.PictureId == model.PictureId)
            ?? throw new ArgumentException("No slides found with the specified picture id");

        slide.TitleText = model.TitleText;
        slide.AltText = model.AltText;
        slide.LinkUrl = model.LinkUrl;

        await SaveSlidesForStoreAsync(slides, storeScope);

        return new NullJsonResult();
    }

    #endregion
}