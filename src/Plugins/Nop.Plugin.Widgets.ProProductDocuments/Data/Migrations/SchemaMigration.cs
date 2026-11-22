using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.ProProductDocuments.Domain;

namespace Nop.Plugin.Widgets.ProProductDocuments.Data.Migrations;

/// <summary>
/// Represents the plugin schema migration
/// </summary>
[NopMigration("2026/11/22 00:00:00:0000000", "Nop.Plugin.Widgets.ProProductDocuments schema", MigrationProcessType.Installation)]
public partial class SchemaMigration : AutoReversingMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<ProductDocument>();
    }

    #endregion
}
