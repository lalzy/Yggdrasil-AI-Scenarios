// IDRules.cs

using AutoBogus;

namespace Yggdrasil.Tests.Factories.BaseRules;

public static class IDRules{
    public static AutoFaker<T> IDRule<T>(this AutoFaker<T> faker) where T:class{
        faker.RuleFor("ID", _ => Guid.Empty);
        return faker;
    }
}
