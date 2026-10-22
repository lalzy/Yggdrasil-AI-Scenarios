// WorldSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class WorldSortOrder : SortOrder{
    public const string DescriptionAsc = "Description.ToLower(), name.ToLower(), ID";
    public const string DescriptionDesc = "Description.ToLower() desc, name.ToLower() desc, ID desc";
}
