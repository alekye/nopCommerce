using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.ProProductDocuments.Domain;

namespace Nop.Plugin.Widgets.ProProductDocuments.Data.Mapping.Builders;

/// <summary>
/// Represents a product document entity builder
/// </summary>
public partial class ProductDocumentBuilder : NopEntityBuilder<ProductDocument>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(ProductDocument.ProductId)).AsInt32().ForeignKey<Product>();
    }

    #endregion
}
