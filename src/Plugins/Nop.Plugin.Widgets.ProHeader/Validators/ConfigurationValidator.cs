using System.Text.RegularExpressions;
using FluentValidation;
using Nop.Plugin.Widgets.ProHeader.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.ProHeader.Validators;

/// <summary>
/// Represents a configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.Text)
            .MaximumLength(4000)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.Text.MaxLength"));

        RuleFor(model => model)
            .Must(model => !model.Enabled || model.BackgroundPictureId > 0 || !string.IsNullOrWhiteSpace(model.Text))
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.Content.Required"));

        RuleFor(model => model.DesktopHeight)
            .InclusiveBetween(0, 600)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.Height.Range"));

        RuleFor(model => model.MobileHeight)
            .InclusiveBetween(0, 400)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.MobileHeight.Range"));

        RuleFor(model => model.OverlayOpacity)
            .InclusiveBetween(0, 80)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.Overlay.Range"));

        RuleFor(model => model.TextColor)
            .Must(value => string.IsNullOrEmpty(value) || Regex.IsMatch(value, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.TextColor.Invalid"));

        RuleFor(model => model.BackgroundColor)
            .Must(value => string.IsNullOrEmpty(value) || Regex.IsMatch(value, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.ProHeader.BackgroundColor.Invalid"));
    }

    #endregion
}
