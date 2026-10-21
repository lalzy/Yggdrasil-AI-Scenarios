// WorldSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class WorldSortOrder : SortOrder{
    public const string DescriptionAsc = "Description, name, ID";
    public const string DescriptionDesc = "Description desc, name desc, ID desc";
}
