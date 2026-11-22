using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.ProProductDocuments.Factories;
using Nop.Plugin.Widgets.ProProductDocuments.Services;

namespace Nop.Plugin.Widgets.ProProductDocuments.Infrastructure;

/// <summary>
/// Represents the plugin dependency registration
/// </summary>
public partial class PluginNopStartup : INopStartup
{
    #region Methods

    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductDocumentService, ProductDocumentService>();
        services.AddScoped<IProductDocumentModelFactory, ProductDocumentModelFactory>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 3000;

    #endregion
}
