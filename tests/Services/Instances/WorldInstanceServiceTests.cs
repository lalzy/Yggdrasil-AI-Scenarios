// WorldInstanceServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Instances;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services.Instances;

public class WorldInstanceServiceTests : DatabaseSetup{
    private readonly WorldInstanceService _service;
    private readonly Faker _faker = new();
    private DbSet<WorldInstance> World => Db().Set<WorldInstance>();

    public WorldInstanceServiceTests(){
        _service = new WorldInstanceService(Db());
    }
    
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesInstance(){
        var template = WorldFactory.Create(Db());

        var fetch = _service.Create(template.ID);
        var dbFetch = World.Find(fetch.ID);

        Assert.NotEqual(template.ID, fetch.ID); // Formality. EFCore throws if the IDs are the same.
        Assert.EquivalentWithExclusions(template, fetch, "ID");
        Assert.Equivalent(fetch, dbFetch);
    }

    [Fact]
    public void Create_Create_InvalidIDReturnsNull(){
        Assert.Null(_service.Create(_faker.Random.Guid()));
    }
    
    // =======================================
    //                  GET              
    // =======================================
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var worlds = Enumerable.Range(0, 10).Select(_ => (WorldFactory.CreateInstance(Db()))).ToList();
        var fetch = _service.Get(worlds[index].ID);

        Assert.Equivalent(worlds[index], fetch);
    }

    [Fact]
    public void Get_ReturnsNullOnInvalid(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }
    
    // =======================================
    //               GET ALL             
    // =======================================
    [Fact]
    public void GetAll_GetAllWorlds(){
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.CreateInstance(Db())).ToList();
        var fetch = _service.GetAll();

        Assert.Equivalent(worlds, fetch, strict:true);
    }



    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count) {
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.CreateInstance(Db())).ToList();
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
    public void GetAll_ReturnsRequestedPage(int page, int count) {
        Enumerable.Range(0, 100).Select(_ => WorldFactory.CreateInstance(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
        var fetch = _service.GetAll(count:count, page:page);

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
    public void GetAll_SortOrderReturned(string order) {
        var worlds = Enumerable.Range(0, 100).Select(_ => WorldFactory.CreateInstance(Db())).ToList();
        var expected = worlds.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder:order);

        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults() {
        var worlds = Enumerable.Range(0, 20).Select(_ => WorldFactory.CreateInstance(Db())).ToList();
        var expected = worlds.AsQueryable().OrderBy(WorldSortOrder.NameAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();

        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    // =======================================
    //                 DELETE              
    // =======================================
    
}
