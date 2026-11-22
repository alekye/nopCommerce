using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.ProProductDocuments.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.ProProductDocuments;

/// <summary>
/// Represents the product documents plugin
/// </summary>
public class ProProductDocumentsPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public ProProductDocumentsPlugin(ILanguageService languageService,
        ILocalizationService localizationService,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _languageService = languageService;
        _localizationService = localizationService;
        _settingService = settingService;
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
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.ProductDetailsBeforeCollateral,
            AdminWidgetZones.ProductDetailsBlock
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
            return typeof(AdminProductDocumentsViewComponent);

        return typeof(ProductDocumentsViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //the plugin renders itself in fixed widget zones and is not managed via the "Widgets" page
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await AddOrUpdateLocaleResourcesAsync();

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
        await AddOrUpdateLocaleResourcesAsync();

        await base.UpdateAsync(currentVersion, targetVersion);
    }

    /// <summary>
    /// Add or update plugin locale resources
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task AddOrUpdateLocaleResourcesAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.ProProductDocuments.Title"] = "Product documents",
            ["Plugins.Widgets.ProProductDocuments.PublicTitle"] = "Downloads",
            ["Plugins.Widgets.ProProductDocuments.ViewDocument"] = "View",
            ["Plugins.Widgets.ProProductDocuments.DownloadDocument"] = "Download",
            ["Plugins.Widgets.ProProductDocuments.Upload"] = "Upload document",
            ["Plugins.Widgets.ProProductDocuments.File"] = "File",
            ["Plugins.Widgets.ProProductDocuments.NoDocuments"] = "No documents have been uploaded yet.",
            ["Plugins.Widgets.ProProductDocuments.SaveProductFirst"] = "Save the product first to be able to add documents.",
            ["Plugins.Widgets.ProProductDocuments.Fields.Title"] = "Title",
            ["Plugins.Widgets.ProProductDocuments.Fields.Title.Hint"] = "Leave empty to use the file name.",
            ["Plugins.Widgets.ProProductDocuments.Fields.DisplayOrder"] = "Display order",
            ["Plugins.Widgets.ProProductDocuments.Fields.IsActive"] = "Is active",
            ["Plugins.Widgets.ProProductDocuments.Fields.File"] = "File",
            ["Plugins.Widgets.ProProductDocuments.FileRequired"] = "Please select a file to upload.",
            ["Plugins.Widgets.ProProductDocuments.FileSizeExceeded"] = "The file size must not exceed {0} MB.",
            ["Plugins.Widgets.ProProductDocuments.FileTypeNotAllowed"] = "This file type is not allowed.",
            ["Plugins.Widgets.ProProductDocuments.Upload.Failed"] = "The document could not be uploaded.",
            ["Plugins.Widgets.ProProductDocuments.Update.Failed"] = "The document could not be updated.",
            ["Plugins.Widgets.ProProductDocuments.Delete.Failed"] = "The document could not be deleted."
        });

        //Chinese translations (applied to existing Chinese languages, if any)
        var chineseResources = new Dictionary<string, string>
        {
            ["Plugins.Widgets.ProProductDocuments.Title"] = "商品文档",
            ["Plugins.Widgets.ProProductDocuments.PublicTitle"] = "资料下载",
            ["Plugins.Widgets.ProProductDocuments.ViewDocument"] = "查看",
            ["Plugins.Widgets.ProProductDocuments.DownloadDocument"] = "下载",
            ["Plugins.Widgets.ProProductDocuments.Upload"] = "上传文档",
            ["Plugins.Widgets.ProProductDocuments.File"] = "文件",
            ["Plugins.Widgets.ProProductDocuments.NoDocuments"] = "暂无文档。",
            ["Plugins.Widgets.ProProductDocuments.SaveProductFirst"] = "请先保存商品，然后再添加文档。",
            ["Plugins.Widgets.ProProductDocuments.Fields.Title"] = "标题",
            ["Plugins.Widgets.ProProductDocuments.Fields.Title.Hint"] = "留空则使用文件名。",
            ["Plugins.Widgets.ProProductDocuments.Fields.DisplayOrder"] = "排序",
            ["Plugins.Widgets.ProProductDocuments.Fields.IsActive"] = "启用",
            ["Plugins.Widgets.ProProductDocuments.Fields.File"] = "文件",
            ["Plugins.Widgets.ProProductDocuments.FileRequired"] = "请选择要上传的文件。",
            ["Plugins.Widgets.ProProductDocuments.FileSizeExceeded"] = "文件大小不能超过 {0} MB。",
            ["Plugins.Widgets.ProProductDocuments.FileTypeNotAllowed"] = "不允许上传该文件类型。",
            ["Plugins.Widgets.ProProductDocuments.Upload.Failed"] = "文档上传失败。",
            ["Plugins.Widgets.ProProductDocuments.Update.Failed"] = "文档更新失败。",
            ["Plugins.Widgets.ProProductDocuments.Delete.Failed"] = "文档删除失败。"
        };

        foreach (var language in (await _languageService.GetAllLanguagesAsync(showHidden: true)).Where(language =>
                     language.LanguageCulture.StartsWith("zh", StringComparison.OrdinalIgnoreCase)))
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(chineseResources, language.Id);
        }
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.ProProductDocuments");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => true;

    #endregion
}
