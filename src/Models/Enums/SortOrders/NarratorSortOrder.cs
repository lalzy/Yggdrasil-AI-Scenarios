// NarratorSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

/// <inheritdoc cref="SortOrder"/>
public class NarratorSortOrder : SortOrder {
    public new const string NameAsc = "Agent.DisplayName, ID";
    public new const string NameDesc = "Agent.DisplayName desc, ID desc";
    public const string DescriptionAsc = "Agent.Description, Agent.DisplayName, ID";
    public const string DescriptionDesc = "Agent.Description desc, Agent.DisplayName desc, ID desc";
    public const string ConnectionAsc = "Connection, Agent.DisplayName,  ID";
    public const string ConnectionDesc = "Connection desc, Agent.DisplayName desc, ID desc";
    public const string InstructionAsc = "Instruction, Agent.DisplayName, ID";
    public const string InstructionDesc = "Instruction desc, Agent.DisplayName desc, ID desc";
}
