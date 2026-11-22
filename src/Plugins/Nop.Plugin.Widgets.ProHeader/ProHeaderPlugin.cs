using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.ProHeader.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.ProHeader;

/// <summary>
/// Represents the pro header bar widget
/// </summary>
public class ProHeaderPlugin : BasePlugin, IWidgetPlugin
{
    #region Constants

    /// <summary>
    /// Gets the default info bar content (copied from the reference site's #header_text block)
    /// </summary>
    public const string DefaultText = """
        <p class="tw-firmenhauptsitz"><strong><span class="tw-bold"><a href="tel:+441536522960"><i aria-hidden="true" class="fa fa-phone"></i> +44 (0) 1536 522 960</a><br><br><a href="mailto:info@twinbusch.co.uk"><i aria-hidden="true" class="fa fa-envelope-o"></i> info@twinbusch.co.uk</a></span><br><br>Twin Busch UK LTD</strong><br>Kettering &#0124; Northants NN16 8PS</p><p class="tw-disappear"><u>Opening hours:</u> <br>Monday-Friday 8 am - 5 pm<br>Saturdays by appointment</p>
        """;

    #endregion

    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public ProHeaderPlugin(ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.BodyStartHtmlTagAfter });
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _webHelper.GetStoreLocation() + "Admin/ProHeader/Configure";
    }

    /// <summary>
    /// Gets a name of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component name</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(ProHeaderViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new ProHeaderSettings
        {
            Enabled = true,
            BackgroundColor = "#f2f2f2",
            Text = DefaultText,
            TextColor = "#2156b0",
            OverlayOpacity = 0,
            DesktopHeight = 220,
            MobileHeight = 120
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.ProHeader.Settings"] = "Info bar settings",
            ["Plugins.Widgets.ProHeader.Enabled"] = "Enabled",
            ["Plugins.Widgets.ProHeader.Enabled.Hint"] = "Check to show the info bar at the top of the home page.",
            ["Plugins.Widgets.ProHeader.BackgroundPictureId"] = "Background picture",
            ["Plugins.Widgets.ProHeader.BackgroundPictureId.Hint"] = "Upload the background picture. A wide picture (for example 2850x450) works best.",
            ["Plugins.Widgets.ProHeader.BackgroundColor"] = "Background color",
            ["Plugins.Widgets.ProHeader.BackgroundColor.Hint"] = "Color shown before the picture is loaded or when no picture is set.",
            ["Plugins.Widgets.ProHeader.Text"] = "Content",
            ["Plugins.Widgets.ProHeader.Text.Hint"] = "Limited HTML is allowed (for example <br>, <strong>, <a>, <i class=\"fa fa-phone\">). The default value reproduces the reference site's contact block.",
            ["Plugins.Widgets.ProHeader.Text.MaxLength"] = "Text must be 4000 characters or fewer.",
            ["Plugins.Widgets.ProHeader.TextColor"] = "Text color",
            ["Plugins.Widgets.ProHeader.TextColor.Hint"] = "Text color in #RRGGBB format.",
            ["Plugins.Widgets.ProHeader.OverlayOpacity"] = "Overlay opacity",
            ["Plugins.Widgets.ProHeader.OverlayOpacity.Hint"] = "Dark overlay over the picture (0-80), useful to improve text contrast.",
            ["Plugins.Widgets.ProHeader.DesktopHeight"] = "Desktop height (px)",
            ["Plugins.Widgets.ProHeader.DesktopHeight.Hint"] = "Bar height on desktop. Set 0 to derive the height from the picture ratio.",
            ["Plugins.Widgets.ProHeader.MobileHeight"] = "Mobile height (px)",
            ["Plugins.Widgets.ProHeader.MobileHeight.Hint"] = "Minimum bar height on screens up to 600px wide.",
            ["Plugins.Widgets.ProHeader.AltText"] = "Picture alternate text",
            ["Plugins.Widgets.ProHeader.AltText.Hint"] = "Added to the picture alt attribute. Leave empty if the picture is decorative.",
            ["Plugins.Widgets.ProHeader.Content.Required"] = "Enter the bar content or choose a background picture when the info bar is enabled.",
            ["Plugins.Widgets.ProHeader.Height.Range"] = "Height must be between 0 and 600 pixels.",
            ["Plugins.Widgets.ProHeader.MobileHeight.Range"] = "Mobile height must be between 0 and 400 pixels.",
            ["Plugins.Widgets.ProHeader.Overlay.Range"] = "Overlay opacity must be between 0 and 80.",
            ["Plugins.Widgets.ProHeader.TextColor.Invalid"] = "Text color must be a valid hex color, for example #ffffff.",
            ["Plugins.Widgets.ProHeader.BackgroundColor.Invalid"] = "Background color must be a valid hex color, for example #4ab2f1.",
            ["Plugins.Widgets.ProHeader.AriaLabel"] = "Store information"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Update plugin
    /// </summary>
    /// <param name="currentVersion">Current version of plugin</param>
    /// <param name="targetVersion">New version of plugin</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        var settings = await _settingService.LoadSettingAsync<ProHeaderSettings>();

        //pre-fill the reference content for installs created before the default was added
        if (string.IsNullOrWhiteSpace(settings.Text))
        {
            settings.Text = DefaultText;
            settings.BackgroundColor = "#f2f2f2";
            settings.TextColor = "#2156b0";
            await _settingService.SaveSettingAsync(settings);
        }

        await base.UpdateAsync(currentVersion, targetVersion);
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<ProHeaderSettings>();

        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locale resources
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.ProHeader");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}
