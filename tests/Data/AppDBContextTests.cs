// AppDbContextTests.cs

using Bogus;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using System.Linq;
using AutoBogus;

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

    [Fact]
    public void SaveChanges_ModifiedEntryWIthoutUpdatedAtDoesNotThrow(){
        var db = Db();
        var entity = new TestEntityNoUpdateAt { Name = _faker.Lorem.Word() };
        db.Add(entity);
        db.SaveChanges();

        entity.Name += "x";
        var ex = Record.Exception(() => db.SaveChanges());
        Assert.Null(ex);
    }

    [Fact]
    public void DeleteCascades_OwnerDeletesOwned(){
        var db = Db();
        var owner = new TestEntityOwner {
            Owned = [
                new TestEntityOwned { Name = _faker.Lorem.Word() },
                new TestEntityOwned { Name = _faker.Lorem.Word() }
            ]
        };
        db.Add(owner);
        db.SaveChanges();

        db.Remove(owner);
        db.SaveChanges();

        Assert.Empty(Db().Set<TestEntityOwned>());
    }
    
    [Fact]
    public void SaveChanges_DuplicateGuidGetsNewId(){
        var id = Guid.NewGuid();

        var first = new TestEntity { ID = id, Name = _faker.Lorem.Word() };
        var db1 = Db();
        db1.Add(first);
        db1.SaveChanges();

        var second = new TestEntity { ID = id, Name = _faker.Lorem.Word() };
        var db2 = Db();
        db2.Add(second);
        db2.SaveChanges();

        Assert.NotEqual(id, second.ID);
        Assert.Equal(2, Db().Set<TestEntity>().Count());
    }

    [Fact]
    public void SaveChanges_ModifiedOwnedUpdatesOwnerUpdatedAt(){
        var db = Db();
        var owned = new TestEntityOwned { Name = _faker.Lorem.Word() };
        var owner = new TestEntityOwner { UpdatedAt = _old, Owned = [owned] };
        db.Add(owner);
        db.SaveChanges();

        owned.Name += "x";
        db.SaveChanges();
        Assert.True(Db().Find<TestEntityOwner>(owner.ID)!.UpdatedAt > _old);
    }
    
    [Fact]
    public void SaveChanges_NewChatUpdatesWorldLastUsed(){
        var db = AppDb();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        world.UpdatedAt = _old;
        db.SaveChanges();

        ChatFactory.Create(db, conversation);

        Assert.True(AppDb().Find<WorldInstance>(world.ID)!.UpdatedAt > _old);
    }

    [Fact]
    public void ConfigureConventions_StringUseNoCaseCollation(){
        var properties = AppDb().GetService<IDesignTimeModel>().Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(string));
        Assert.All(properties, p => Assert.Equal("NOCASE", p.GetCollation()));
    }

    public void Dispose() => _connection.Dispose();
}
