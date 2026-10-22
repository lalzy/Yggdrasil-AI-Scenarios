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

    [Fact]
    public void Initialize_NotOverwrittenOnRerun(){
        var word = _faker.Random.Word();
        App.Initialize();
        var db = Db();
        var settings = db.Set<Settings>().Include(s => s.ActivePrompt).Single();
        settings.ActivePrompt.Name = word;
        db.SaveChanges();

        App.Initialize();
        var result = Db().Set<Settings>().Include(s => s.ActivePrompt).Single();
        Assert.Equal(word, result.ActivePrompt.Name);
    }
}
