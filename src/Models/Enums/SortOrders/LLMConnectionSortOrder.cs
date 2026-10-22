// LLMConnectionSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class LLMConnectionSortOrder : SortOrder{
    public const string URLAsc = "URL.ToLower(), ID";
    public const string URLDesc = "URL.ToLower() desc, ID desc";
    public const string ModelAsc = "URL.ToLower(), Name.ToLower(), ID";
    public const string ModelDesc = "URL.ToLower() desc, Name.ToLower() desc, ID desc";
}
