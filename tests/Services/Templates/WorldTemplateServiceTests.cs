// WorldTemplateServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services.Templates;

public class WorldTemplateServiceTests : DatabaseSetup{
    private readonly WorldTemplateService _service;
    private readonly Faker _faker = new();
    private DbSet<WorldTemplate> World => Db().Set<WorldTemplate>();

    public WorldTemplateServiceTests(){
        _service = new WorldTemplateService(Db());
    }
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesWorld(){
        var narrator = NarratorFactory.Create(Db());
        var request = new AutoFaker<WorldDTO.Request>().RuleFor(x=>x.NarratorID, _ => narrator.ID).Generate();
        
        // var request = AutoFaker.Generate<WorldDTO.Request>();
        var fetch = _service.Create(request);

        Assert.EquivalentWithExclusions(request, fetch, "NarratorID", "Narrator");

        var dbFetch = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).First(w => w.ID == fetch.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "NarratorID", "Narrator");
        
        // Has Narrator
        Assert.Equivalent(narrator, fetch.Narrator);
        Assert.Equivalent(narrator, dbFetch.Narrator);
    }

    [Fact]
    public void Create_TagsAndOpenMessageReturnEmptyIfNotPassed(){
        var narrator = NarratorFactory.Create(Db());
        var request = new AutoFaker<WorldDTO.Request>().RuleFor(x=>x.NarratorID, _ => narrator.ID)
            .RuleFor(x => x.Tags,_=> null)
            .RuleFor(x => x.OpeningMessages, _ => null).Generate();
        var fetch = _service.Create(request);
        Assert.Empty(fetch.Tags);
        Assert.Empty(fetch.OpeningMessages);
    }

    [Fact]
    public void Create_InvalidNarratorThrows(){
        var request = AutoFaker.Generate<WorldDTO.Request>();
        Assert.Throws<KeyNotFoundException>(() => _service.Create(request));
    }
    
    // =======================================
    //                  GET              
    // =======================================
    
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var worlds = Enumerable.Range(0, 10).Select(_ => WorldFactory.Create(Db())).ToList();
        var fetch = _service.Get(worlds[index].ID);

        Assert.Equivalent(worlds[index], fetch);
    }

    [Fact]
    public void Get_InvalidIDReturnNull(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }
    
    // =======================================
    //               GET ALL             
    // =======================================

    [Fact]
    public void GetAll_GetAllWorlds(){
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(worlds, fetch, strict:true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.Create(Db())).ToList();
        var expectedIDs = worlds.Select(w => w.ID).ToHashSet();
        var fetch = _service.GetAll(count:count);

        Assert.Equal(count, fetch.Count);
        Assert.All(fetch, w => Assert.Contains(w.ID, expectedIDs));
        
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 10)]
    [InlineData(3, 30)]
    [InlineData(9, 10)]
    public void GetAll_ReturnsRequestedPage(int page, int count){
        Enumerable.Range(0, 100).Select(_ => WorldFactory.Create(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
        var fetch = _service.GetAll(count: count, page:page);

        Assert.Equal(expected, fetch.Select(w => w.ID));
    }

    [Theory]
    [InlineData(WorldSortOrder.IDAsc)]
    [InlineData(WorldSortOrder.IDDesc)]
    [InlineData(WorldSortOrder.NameAsc)]
    [InlineData(WorldSortOrder.NameDesc)]
    [InlineData(WorldSortOrder.DescriptionAsc)]
    [InlineData(WorldSortOrder.DescriptionDesc)]
    [InlineData(WorldSortOrder.ModifiedAsc)]
    [InlineData(WorldSortOrder.ModifiedDesc)]
    [InlineData(WorldSortOrder.CreatedAsc)]
    [InlineData(WorldSortOrder.CreatedDesc)]
    public void GetAll_SortOrderReturned(string order){
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.Create(Db())).ToList();
        var expected = worlds.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);
        
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults(){
        var worlds = Enumerable.Range(0, 20).Select(_ => WorldFactory.Create(Db())).ToList();
        var expected = worlds.AsQueryable().OrderBy(WorldSortOrder.NameAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();
        
        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var world = WorldFactory.Create(Db());
        var request = EnsureUnique.Request<WorldDTO.Update, WorldTemplate>(world) with {NarratorID = null};
        var fetch = _service.Update(world.ID, request);
        
        Assert.EquivalentWithExclusions(request, fetch, "Narrator", "NarratorID");

        var dbFetch = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).First(w => w.ID == world.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "Narrator", "NarratorID");

        Assert.Equivalent(dbFetch.Narrator, fetch.Narrator, strict:true);
    }

    [Fact]
    public void Update_ReturnsNarrator(){
        var world = WorldFactory.Create(Db());
        var request = EnsureUnique.Request<WorldDTO.Update, WorldTemplate>(world) with {NarratorID = null};
        var fetch = _service.Update(world.ID, request);
        var dbFetch = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).First(w => w.ID == world.ID);
        Assert.Equivalent(dbFetch, fetch, strict:true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var worlds = Enumerable.Range(0, 10).Select(_ => WorldFactory.Create(Db())).ToList();
        var request = EnsureUnique.Request<WorldDTO.Update, WorldTemplate>(worlds[index]) with {NarratorID = null};

        _service.Update(worlds[index].ID, request);
        var expected = worlds.Where(w => w.ID != worlds[index].ID).ToList();
        var actual = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).Where(w => w.ID != worlds[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var world = WorldFactory.Create(Db());
        var fullRequest = EnsureUnique.Request<WorldDTO.Update, WorldTemplate>(world);
        var properties = fullRequest.GetType().GetProperties()
            .Where(p => p.Name != nameof(WorldDTO.Update.NarratorID)) // Skip narrator
            .ToArray();
        foreach(var property in properties){
            var before = properties.ToDictionary(
                p => p.Name,
                p => typeof(WorldTemplate).GetProperty(p.Name)!.GetValue(World.Find(world.ID))
            );

            _service.Update(world.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

            var dbFetch = World.Find(world.ID);
            foreach(var p in properties){
                var actual = typeof(WorldTemplate).GetProperty(p.Name)!.GetValue(dbFetch);
                var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
                Assert.Equivalent(expected, actual);
            }
        }
    }

    [Fact]
    public void Update_ChangeNarrator(){
        var db = Db();
        var oldNarrator = NarratorFactory.Create(db);
        var newNarrator = NarratorFactory.Create(db);
        var world = WorldFactory.Create(db, oldNarrator);
        var request = new WorldDTO.Update{NarratorID = newNarrator.ID};
        _service.Update(world.ID, request);
        var dbFetch = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).First(w => w.ID == world.ID);
        Assert.Equal(dbFetch.Narrator.ID, newNarrator.ID);
    }

    [Fact]
    public void Update_InvalidNarratorThrows(){
        var world = WorldFactory.Create(Db());
        var request = new WorldDTO.Update{NarratorID = _faker.Random.Guid()};
        Assert.Throws<KeyNotFoundException>(() => _service.Update(world.ID, request));
    }

    [Fact]
    public void Update_InvalidIDReturnNull(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<WorldDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_Successfull(){
        var world = WorldFactory.Create(Db());
        _service.Delete(world.ID);
        Assert.Null(World.Find(world.ID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_OnlyRequestedDeleted(int index){
        var worlds = Enumerable.Range(0, 10).Select(_ => WorldFactory.Create(Db())).ToList();
        _service.Delete(worlds[index].ID);

        var expected = worlds.Where(w => w.ID != worlds[index].ID).ToList();
        var actual = World.Include(w => w.Narrator).ThenInclude(n => n.Connection).Where(w => w.ID != worlds[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
    }
}
