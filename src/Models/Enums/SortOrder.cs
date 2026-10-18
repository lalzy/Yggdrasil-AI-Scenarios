// SortOrder.cs

namespace Yggdrasil.Models.Enums;

public class SortOrder
{
    private static int _next;
    public int Value { get; } = _next++;

    public static readonly SortOrder NameAsc = new();
    public static readonly SortOrder NameDesc = new();
    public static readonly SortOrder CreatedAsc = new();
    public static readonly SortOrder CreatedDesc = new();
    public static readonly SortOrder ModifiedAsc = new();
    public static readonly SortOrder ModifiedDesc = new();
    public static readonly SortOrder IDAsc = new();
    public static readonly SortOrder IDDesc = new();
}

public class WorldSortOrder : SortOrder{
    public static readonly WorldSortOrder DescriptionAsc = new();
}
