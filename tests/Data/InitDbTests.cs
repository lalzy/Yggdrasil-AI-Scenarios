// InitDbTests.cs

using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Tests.TestUtil;
using Bogus;

namespace Yggdrasil.Tests.Data;

public class InitDbTests : DatabaseSetup
{
    private readonly Faker _faker = new();
    [Fact]
    public void Initialize_CreateSettings()
    {
        App.Initialize();
        Assert.Equal(1, Db().Set<Settings>().Count());
    }

    [Fact]
    public void Initialize_OnlyOneSettingsRow(){
        App.Initialize();
        App.Initialize();
        Assert.Equal(1, Db().Set<Settings>().Count());
    }
}
