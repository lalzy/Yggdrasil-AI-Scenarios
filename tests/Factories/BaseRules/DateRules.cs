// DateRules.cs

using AutoBogus;

namespace Yggdrasil.Tests.Factories.BaseRules;

public static class DateRules{
    public static AutoFaker<T> CreateAndUpdateAtRule<T>(this AutoFaker<T> faker) where T : class{
        faker.RuleFor("CreatedAt", _ => DateTime.UtcNow)
            .RuleFor("UpdatedAt", _=> DateTime.UtcNow);
        return faker;
    }
}
