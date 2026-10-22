// NarratorSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class NarratorSortOrder : SortOrder {
    public new const string NameAsc = "Agent.DisplayName.ToLower(), ID";
    public new const string NameDesc = "Agent.DisplayName.ToLower() desc, ID desc";
    public const string DescriptionAsc = "Agent.Description.ToLower(), Agent.DisplayName.ToLower(), ID";
    public const string DescriptionDesc = "Agent.Description.ToLower() desc, Agent.DisplayName.ToLower() desc, ID desc";
    public const string ConnectionAsc = "Connection, Agent.DisplayName.ToLower(), ID";
    public const string ConnectionDesc = "Connection desc, Agent.DisplayName.ToLower() desc, ID desc";
}
