/// Settings.cs

namespace Yggdrasil.Models.Entities;

public enum Themes{
    dark=0,
    light=1,
}

public class Settings{
    public Guid ID {get; set;} = Guid.Empty;
    public Themes Theme {get;set;} = Themes.dark;
}
