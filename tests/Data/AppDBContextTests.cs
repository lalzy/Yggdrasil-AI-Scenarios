// AppDbContextTests.cs

using Bogus;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data;
using Yggdrasil.Test.TestUtil;
using Yggdrasil.Models.Entities;

namespace Yggdrasil.Tests.Data;

public class AppDbContextTests : IDisposable{
    private readonly SqliteConnection _connection;
    private readonly DateTime _old = new(2000, 1, 1);
    private readonly Faker _faker = new();

    private AppDbContext AppDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

    public AppDbContextTests(){
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        Db().Database.EnsureCreated();
    }
    
    public TestDBContext Db() => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

    [Fact]
    public void SaveChanges_ModifiedEntryUpdatedAt(){
        var db = Db();
        var entity = new TestEntity { Name = _faker.Lorem.Word(), UpdatedAt = _old };
        db.Add(entity);
        db.SaveChanges();

        entity.Name = "s";
        db.SaveChanges();
        Assert.True(Db().Find<TestEntity>(entity.ID)!.UpdatedAt > _old);
    }

    [Fact]
    public void SaveChanges_UnmodifiedDoesNotChangeUpdatedAt(){
        var db = Db();
        var modified = new TestEntity { Name = "a", UpdatedAt = _old };
        var untouched = new TestEntity { Name = "a", UpdatedAt = _old };
        db.AddRange(modified, untouched);
        db.SaveChanges();

        modified.Name = "b";
        db.SaveChanges();
        Assert.Equal(_old, Db().Find<TestEntity>(untouched.ID)!.UpdatedAt);
    }

    [Fact]
    public void OnModelCreating_RegistersAllEntities(){
        var model = AppDb().Model;
        var expected = typeof(AppDbContext).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract
                                                                      && t.Namespace == "Yggdrasil.Models.Entities"
                                                                      && t != typeof(Prompt));
        Assert.All(expected, t => Assert.NotNull(model.FindEntityType(t)));
    }

    [Fact]
    public void OnModelCreating_PromptIsNotATable(){
        var model = AppDb().Model;
        Assert.DoesNotContain(model.GetEntityTypes(), e => e.ClrType == typeof(Prompt) && !e.IsOwned());
    }

    [Fact]
    public void OnModelCreating_PromptsStoredAsJson(){
        var nav = AppDb().Model.FindEntityType(typeof(SystemPrompt))!
            .FindNavigation(nameof(SystemPrompt.Prompts))!;

        Assert.True(nav.TargetEntityType.IsMappedToJson());
    }
    
    public void Dispose() => _connection.Dispose();
}
