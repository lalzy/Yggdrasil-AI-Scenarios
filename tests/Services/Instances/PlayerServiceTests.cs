// PlayerServiceTests.cs

using AutoBogus;
using System.Linq.Dynamic.Core;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Tests.Factories;

namespace Yggdrasil.Tests.Services.Instances;

public class PlayerServiceTests : DatabaseSetup{
    private readonly PlayerService _service;
    private readonly Faker _faker = new();
    private DbSet<PersonaTemplate> Templates => Db().Set<PersonaTemplate>();
    private DbSet<PlayerInstance> Players => Db().Set<PlayerInstance>();

    public PlayerServiceTests(){
        _service = new PlayerService(Db());
    }

    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesPersona(){
        var template = PersonaFactory.Create(Db());
        var fetch = _service.Create(template.ID);
        var dbFetch = Players.Find(fetch!.ID);
        
        Assert.NotEqual(template.ID, fetch.ID);
        Assert.EquivalentWithExclusions(template, fetch, "ID");
        Assert.Equivalent(fetch, dbFetch);
    }
    
    [Fact]
    public void Create_InvalidIDReturnsNull(){
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
        var personas = Enumerable.Range(0, 10).Select(_ => PlayerFactory.Create(Db())).ToList();
        var fetch = _service.Get(personas[index].ID);

        Assert.Equivalent(personas[index], fetch);
    }

    [Fact]
    public void Get_InvalidIDReturnNull(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }
    
    // =======================================
    //               GET ALL             
    // =======================================

    [Fact]
    public void GetAll_GetAllCharacters(){
        var players = Enumerable.Range(0, 100).Select(_ => PlayerFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(players, fetch, strict:true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var players = Enumerable.Range(0, 100).Select(_ => PlayerFactory.Create(Db())).ToList();
        var expectedIDs = players.Select(w => w.ID).ToHashSet();
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
        Enumerable.Range(0, 100).Select(_ => PlayerFactory.Create(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
        var fetch = _service.GetAll(count: count, page:page);

        Assert.Equal(expected, fetch.Select(w => w.ID));
    }

    [Theory]
    [InlineData(CharacterSortOrder.IDAsc)]
    [InlineData(CharacterSortOrder.IDDesc)]
    [InlineData(CharacterSortOrder.DisplayNameAsc)]
    [InlineData(CharacterSortOrder.DisplayNameDesc)]
    [InlineData(CharacterSortOrder.NameAsc)]
    [InlineData(CharacterSortOrder.NameDesc)]
    [InlineData(CharacterSortOrder.DescriptionAsc)]
    [InlineData(CharacterSortOrder.DescriptionDesc)]
    [InlineData(CharacterSortOrder.ModifiedAsc)]
    [InlineData(CharacterSortOrder.ModifiedDesc)]
    [InlineData(CharacterSortOrder.CreatedAsc)]
    [InlineData(CharacterSortOrder.CreatedDesc)]
    public void GetAll_SortOrderReturned(string order){
        var players = Enumerable.Range(0, 100).Select(_ => PlayerFactory.Create(Db())).ToList();
        var expected = players.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);
        
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults(){
        var players = Enumerable.Range(0, 20).Select(_ => PlayerFactory.Create(Db())).ToList();
        var expected = players.AsQueryable().OrderBy(CharacterSortOrder.DisplayNameAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();
        
        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var player = PlayerFactory.Create(Db());
        var request = EnsureUnique.Request<PlayerDTO.Update, PlayerInstance>(player);
        var fetch = _service.Update(player.ID, request);
        
        Assert.Equivalent(request, fetch, strict:true);

        var dbFetch = Players.Find(player.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, strict:true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var players = Enumerable.Range(0, 10).Select(_ => PlayerFactory.Create(Db())).ToList();
        var request = EnsureUnique.Request<PlayerDTO.Update, PlayerInstance>(players[index]);

        _service.Update(players[index].ID, request);
        var expected = players.Where(w => w.ID != players[index].ID).ToList();
        var actual = Players.Where(w => w.ID != players[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var player = PlayerFactory.Create(Db());
        var fullRequest = EnsureUnique.Request<PlayerDTO.Update, PlayerInstance>(player);
        var properties = fullRequest.GetType().GetProperties().ToArray();
        foreach(var property in properties){
            var before = properties.ToDictionary(
                p => p.Name,
                p => typeof(CharacterTemplate).GetProperty(p.Name)!.GetValue(Players.Find(player.ID))
            );

            _service.Update(player.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

            var dbFetch = Players.Find(player.ID);
            foreach(var p in properties){
                var actual = typeof(CharacterTemplate).GetProperty(p.Name)!.GetValue(dbFetch);
                var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
                Assert.Equivalent(expected, actual);
            }
        }
    }

    [Fact]
    public void Update_OnlyPassedNestedFieldsChanged(){
        var player = PlayerFactory.Create(Db());
        Nesting.AssertOnlyPassedFieldsChanged(
            player,
            () => Players.Find(player.ID),
            (PlayerDTO.Update r) => _service.Update(player.ID, r),
            r => r.Profile!,
            c => c!.Profile,
            d => new PlayerDTO.Update{ Profile = d });
    }

    [Fact]
    public void Update_InvalidIDReturnNull(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<PlayerDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_Successfull(){
        var player = PlayerFactory.Create(Db());
        _service.Delete(player.ID);
        Assert.Null(Players.Find(player.ID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_OnlyRequestedDeleted(int index){
        var players = Enumerable.Range(0, 10).Select(_ => PlayerFactory.Create(Db())).ToList();
        _service.Delete(players[index].ID);

        var expected = players.Where(w => w.ID != players[index].ID).ToList();
        var actual = Players.Where(w => w.ID != players[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
    }
}
