// AppDbContextTests.cs

using Bogus;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

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
    public void OnModelCreating_RegistersAllDesiredEntities(){
        var model = AppDb().Model;
        var expected = typeof(AppDbContext).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                && AppDbContext.IsInNamespace(t));
        Assert.All(expected, t => Assert.NotNull(model.FindEntityType(t)));
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

    [Fact]
    public void OnModelCreating_ComplexTypesAreNotTables(){
        var model = AppDb().Model;
        var valueObjects = typeof(AppDbContext).Assembly.GetTypes()
            .Where(t => t.IsClass && t.Namespace == "Yggdrasil.Models.ValueObjects");
        Assert.All(valueObjects, t => Assert.Null(model.FindEntityType(t)));
    }

    [Fact]
    public void OnModelCreating_ComplexCollectionsStoredAsJson(){
        var entities = AppDb().Model.GetEntityTypes();
        var props = entities
            .SelectMany(e => e.ClrType.GetProperties(), (e, p) => (e, p))
            .Where(x => x.p.PropertyType.IsGenericType
                && x.p.PropertyType.GetGenericArguments()[0].GetCustomAttribute<ComplexTypeAttribute>() is not null);

        Assert.All(props, x =>
            Assert.True(x.e.FindComplexProperty(x.p.Name)?.ComplexType.IsMappedToJson() == true,
                $"{x.e.ClrType.Name}.{x.p.Name}"));
    }

    [Fact]
    public void OnModelCreating_SingleComplexTypesNotJson(){
        var props = AppDb().Model.GetEntityTypes()
            .SelectMany(e => e.GetComplexProperties())
            .Where(p => !p.IsCollection);

        Assert.All(props, p =>
            Assert.False(p.ComplexType.IsMappedToJson(), $"{p.DeclaringType.ClrType.Name}.{p.Name}"));
    }

    [Fact]
    public void SaveChanges_ModifiedNarratorDoesNotUpdateConnectionUpdatedAt(){
        var db = AppDb();
        var oldConnection = LLMConnectionFactory.Create(db);
        var newConnection = LLMConnectionFactory.Create(db);
        var narrator = NarratorFactory.Create(db, oldConnection);
        var expected = AppDb().Find<LLMConnection>(newConnection.ID)!.UpdatedAt;

        narrator.Connection = newConnection;
        db.SaveChanges();

        Assert.Equal(expected, AppDb().Find<LLMConnection>(newConnection.ID)!.UpdatedAt);
    }

    public void Dispose() => _connection.Dispose();
}
