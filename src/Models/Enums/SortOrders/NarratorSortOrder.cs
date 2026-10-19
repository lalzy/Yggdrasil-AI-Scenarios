// NarratorSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class NarratorSortOrder : SortOrder {
    public new const string NameAsc = "DisplayName, ID";
    public new const string NameDesc = "DisplayName desc, ID";
    public const string DescriptionAsc = "Description, DisplayName, ID";
    public const string DescriptionDesc = "Description desc, DisplayName desc, ID";
    public const string ConnectionAsc = "Connection, DisplayName,  ID";
    public const string ConnectionDesc = "Connection desc, DisplayName desc, ID";
    public const string InstructionAsc = "Instruction, DisplayName, ID";
    public const string InstructionDesc = "Instruction desc, DisplayName desc, ID";
}
