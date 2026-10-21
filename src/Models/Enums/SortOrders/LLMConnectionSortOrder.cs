// LLMConnectionSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class LLMConnectionSortOrder : SortOrder{
    public const string URLAsc = "URL, ID";
    public const string URLDesc = "URL desc, ID desc";
    public const string ModelAsc = "URL, Name, ID";
    public const string ModelDesc = "URL desc, Name desc, ID desc";
}
