// CharacterSortOrder

namespace Yggdrasil.Models.Enums.SortOrders;

public class CharacterSortOrder : SortOrder{
    public new const string NameAsc = "Profile.FirstName, Profile.LastName, ID";
    public new const string NameDesc = "Profile.FirstName desc, Profile.LastName desc, ID desc";
    public const string DisplayNameAsc = "Agent.DisplayName, ID";
    public const string DisplayNameDesc = "Agent.DisplayName desc, ID desc";
    public const string DescriptionAsc = "Agent.Description, Profile.FirstName, Profile.LastName, ID";
    public const string DescriptionDesc = "Agent.Description desc, Profile.FirstName desc, Profile.LastName desc, ID desc";
    public const string GenderAsc = "Profile.Gender, Profile.FirstName, Profile.LastName, ID";
    public const string GenderDesc = "Profile.Gender desc, Profile.FirstName desc, Profile.LastName desc, ID desc";
    public const string RaceAsc = "Profile.Race, Profile.FirstName, Profile.LastName, ID";
    public const string RaceDesc = "Profile.Race desc, Profile.FirstName desc, Profile.LastName desc, ID desc";
}
