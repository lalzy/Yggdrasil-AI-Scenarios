// SortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <summary>Smart enum for EntityFrameworkCore.DynamicLinq</summary>
public class SortOrder{
    public const string IDAsc        = "ID";
    public const string IDDesc       = "ID desc";
    public const string NameAsc      = "Name, ID";
    public const string NameDesc     = "Name desc, ID";
    public const string CreatedAsc   = "CreatedAt, ID";
    public const string CreatedDesc  = "CreatedAt desc, ID";
    public const string ModifiedAsc  = "UpdatedAt, ID";
    public const string ModifiedDesc = "UpdatedAt desc, ID";
}
