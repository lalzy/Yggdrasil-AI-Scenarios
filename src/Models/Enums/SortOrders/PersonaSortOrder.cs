// PersonaSortOrder.cs

namespace Yggdrasil.Models.Enums.SortOrders;

public class PersonaSortOrder : SortOrder{
    public new const string NameAsc = "Profile.FirstName.ToLower(), Profile.LastName.ToLower(), ID";
    public new const string NameDesc = "Profile.FirstName.ToLower() desc, Profile.LastName.ToLower() desc, ID desc";
    public const string DisplayNameAsc = "DisplayName.ToLower(), ID";
    public const string DisplayNameDesc = "DisplayName.ToLower() desc, ID desc";
    public const string DescriptionAsc = "Description.ToLower(), Profile.FirstName.ToLower(), Profile.LastName.ToLower(), ID";
    public const string DescriptionDesc = "Description.ToLower() desc, Profile.FirstName.ToLower() desc, Profile.LastName.ToLower() desc, ID desc";
    public const string GenderAsc = "Profile.Gender.ToLower(), Profile.FirstName.ToLower(), Profile.LastName.ToLower(), ID";
    public const string GenderDesc = "Profile.Gender.ToLower() desc, Profile.FirstName.ToLower() desc, Profile.LastName.ToLower() desc, ID desc";
    public const string RaceAsc = "Profile.Race.ToLower(), Profile.FirstName.ToLower(), Profile.LastName.ToLower(), ID";
    public const string RaceDesc = "Profile.Race.ToLower() desc, Profile.FirstName.ToLower() desc, Profile.LastName.ToLower() desc, ID desc";
}
