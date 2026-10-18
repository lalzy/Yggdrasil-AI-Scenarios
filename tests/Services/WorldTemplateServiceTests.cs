// WorldTemplateServiceTests.cs
using FluentAssertions;
using AutoBogus;
using Bogus;
using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums;
using Yggdrasil.Data;
using System.Runtime.CompilerServices;

namespace Yggdrasil.Tests.Services;

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
        var request = AutoFaker.Generate<WorldTemplateDTO.Request>();
        var fetch = _service.Create(request);

        Assert.Equivalent(request, fetch);

        var dbFetch = World.Find(fetch.ID);
        Assert.Equivalent(request, dbFetch);
    }

    [Fact]
    public void Create_TagsAndOpenMessageReturnEmptyIfNotPassed(){
        var request = new AutoFaker<WorldTemplateDTO.Request>()
            .RuleFor(x => x.Tags,_=> null)
            .RuleFor(x => x.OpeningMessages, _ => null).Generate();
        var fetch = _service.Create(request);
        Assert.Empty(fetch.Tags);
        Assert.Empty(fetch.OpeningMessages);
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

    private WorldTemplateDTO.Update EnsureUniqueUpdateRequest(WorldTemplate world){
        var properties = typeof(WorldTemplateDTO.Update).GetProperties();
        WorldTemplateDTO.Update request;
        do{
            request = AutoFaker.Generate<WorldTemplateDTO.Update>();
        }while(properties.Any(p => Equals(p.GetValue(request), typeof(WorldTemplate).GetProperty(p.Name)?.GetValue(world))));
        return request;
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

    public static TheoryData<string, string, bool> SortCases => new(){
        {WorldSortOrder.IDAsc, "ID", false},
        {WorldSortOrder.IDDesc, "ID", true},
        {WorldSortOrder.NameAsc, "Name", false},
        {WorldSortOrder.NameDesc, "Name", true},
        {WorldSortOrder.ModifiedAsc, "UpdatedAt", false},
        {WorldSortOrder.ModifiedDesc, "UpdatedAt", true},
        {WorldSortOrder.CreatedAsc, "CreatedAt", false},
        {WorldSortOrder.CreatedDesc, "CreatedAt", true},
        {WorldSortOrder.DescriptionAsc, "Description", false},
        {WorldSortOrder.DescriptionDesc, "Description", true},
    };

    [Theory]
    [MemberData(nameof(SortCases))]
    public void GetAll_SortOrderReturned(string order, string propertyToSortBy, bool descendingFlag){
        var worlds = Enumerable.Range(0, 100).Select(_=> WorldFactory.Create(Db())).ToList();
        var property = typeof(WorldTemplate).GetProperty(propertyToSortBy);

        var comparer = Comparer<object>.Create(StringComparer.OrdinalIgnoreCase.Compare);

        var expected = descendingFlag ? worlds.OrderByDescending(w => property.GetValue(w), comparer!).ThenBy(w => w.ID) : worlds.OrderBy(w => property!.GetValue(w), comparer!).ThenBy(w => w.ID);

        var fetch = _service.GetAll(sortOrder:order);
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }
    
    // =======================================
    //                UPDATE              
    // =======================================
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var world = WorldFactory.Create(Db());
        var request = EnsureUniqueUpdateRequest(world);
        var fetch = _service.Update(world.ID, request);
        
        Assert.Equivalent(request, fetch);

        var dbFetch = World.Find(world.ID);
        Assert.Equivalent(request, dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var worlds = Enumerable.Range(0, 10).Select(_ => WorldFactory.Create(Db())).ToList();
        var request = EnsureUniqueUpdateRequest(worlds[index]);
        _service.Update(worlds[index].ID, request);
        var expected = worlds.Where(w => w.ID != worlds[index].ID).ToList();
        var actual = World.Where(w => w.ID != worlds[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var world = WorldFactory.Create(Db());
        var fullRequest = EnsureUniqueUpdateRequest(world);
        var properties = fullRequest.GetType().GetProperties();
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
    public void Update_InvalidIDReturnNUll(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<WorldTemplateDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================

}
