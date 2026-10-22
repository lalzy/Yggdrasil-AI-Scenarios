// CharacterTemplateServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using System.Runtime.CompilerServices;
using System.Linq.Dynamic.Core;
using Yggdrasil.Models.ValueObjects;
using System.Collections;
using System.Reflection;

using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.ValueObjects;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using System.Runtime.CompilerServices;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services;

public class CharacterTemplateServiceTests : DatabaseSetup{
    private readonly CharacterTemplateService _service;
    private readonly Faker _faker = new();
    private DbSet<CharacterTemplate> Characters => Db().Set<CharacterTemplate>();

    public CharacterTemplateServiceTests(){
        _service = new CharacterTemplateService(Db());
    }

    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesCharacter(){
        var request = new AutoFaker<CharacterDTO.Request>()
            .RuleFor(x => x.ConnectionID, _ => null)
            .Generate();
        
        var fetch = _service.Create(request);

        Assert.EquivalentWithExclusions(request, fetch, "ConnectionID", "Connection");

        var dbFetch = Characters.Include(c => c.Connection).First(w => w.ID == fetch.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "ConnectionID", "Connection");
    }

    [Fact]
    public void Create_AddPassingConnection(){
        var connection = LLMConnectionFactory.Create(Db());
        var request = new AutoFaker<CharacterDTO.Request>()
            .RuleFor(x => x.ConnectionID, _ => connection.ID)
            .Generate();
        var fetch = _service.Create(request);
        var dbFetch = Characters.Include(c => c.Connection).First(c => c.ID == fetch.ID);
        Assert.Equivalent(connection, fetch.Connection, strict:true);
        Assert.Equivalent(connection, dbFetch.Connection, strict:true);
    }
    
    [Fact]
    public void Create_InvalidConnectionThrows(){
        var request = AutoFaker.Generate<CharacterDTO.Request>();
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
        var characters = Enumerable.Range(0, 10).Select(_ => CharacterFactory.Create(Db())).ToList();
        var fetch = _service.Get(characters[index].ID);

        Assert.Equivalent(characters[index], fetch);
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
        var characters = Enumerable.Range(0, 100).Select(_ => CharacterFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(characters, fetch, strict:true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var characters = Enumerable.Range(0, 100).Select(_ => CharacterFactory.Create(Db())).ToList();
        var expectedIDs = characters.Select(w => w.ID).ToHashSet();
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
        Enumerable.Range(0, 100).Select(_ => CharacterFactory.Create(Db())).ToList();
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
        var characters = Enumerable.Range(0, 100).Select(_ => CharacterFactory.Create(Db())).ToList();
        var expected = characters.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);
        
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults(){
        var characters = Enumerable.Range(0, 20).Select(_ => CharacterFactory.Create(Db())).ToList();
        var expected = characters.AsQueryable().OrderBy(CharacterSortOrder.DisplayNameAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();
        
        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var character = CharacterFactory.Create(Db());
        var request = EnsureUnique.Request<CharacterDTO.Update, CharacterTemplate>(character) with {ConnectionID = null};
        var fetch = _service.Update(character.ID, request);
        
        Assert.EquivalentWithExclusions(request, fetch, "Connection", "ConnectionID");

        var dbFetch = Characters.Include(c => c.Connection).First(w => w.ID == character.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "Connection", "ConnectionID");

        Assert.Equivalent(dbFetch.Connection, fetch.Connection, strict:true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var characters = Enumerable.Range(0, 10).Select(_ => CharacterFactory.Create(Db())).ToList();
        var request = EnsureUnique.Request<CharacterDTO.Update, CharacterTemplate>(characters[index]) with {ConnectionID = null};

        _service.Update(characters[index].ID, request);
        var expected = characters.Where(w => w.ID != characters[index].ID).ToList();
        var actual = Characters.Include(c => c.Connection).Where(w => w.ID != characters[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var character = CharacterFactory.Create(Db());
        var fullRequest = EnsureUnique.Request<CharacterDTO.Update, CharacterTemplate>(character) with {ConnectionID = null};
        var properties = fullRequest.GetType().GetProperties()
            .Where(p => p.Name != nameof(CharacterDTO.Update.ConnectionID)) // Skip Connection
            .ToArray();
        foreach(var property in properties){
            var before = properties.ToDictionary(
                p => p.Name,
                p => typeof(CharacterTemplate).GetProperty(p.Name)!.GetValue(Characters.Find(character.ID))
            );

            _service.Update(character.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

            var dbFetch = Characters.Find(character.ID);
            foreach(var p in properties){
                var actual = typeof(CharacterTemplate).GetProperty(p.Name)!.GetValue(dbFetch);
                var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
                Assert.Equivalent(expected, actual);
            }
        }
    }

    [Theory]
    [InlineData("Profile")]
    [InlineData("Agent")]
    public void Update_OnlyPassedNestedFieldsChanged(string nestedProperty){
        var character = CharacterFactory.Create(Db());

        switch(nestedProperty){
            case "Profile":
                Nesting.AssertOnlyPassedFieldsChanged(
                    character,
                    () => Characters.Find(character.ID),
                    (CharacterDTO.Update r) => _service.Update(character.ID, r),
                    r => r.Profile!,
                    c => c.Profile,
                    d => new CharacterDTO.Update{ Profile = d });
                break;
                
            case "Agent":
                Nesting.AssertOnlyPassedFieldsChanged(
                    character,
                    () => Characters.Find(character.ID),
                    (CharacterDTO.Update r) => _service.Update(character.ID, r),
                    r => r.Agent!,
                    c => c.Agent,
                    d => new CharacterDTO.Update {Agent = d});
                break;
        };
    }

    [Fact]
    public void Update_ChangeConnection(){
        var db = Db();
        var connection = LLMConnectionFactory.Create(db);
        var character = CharacterFactory.Create(db);
        var request = new CharacterDTO.Update{ConnectionID = connection.ID};
        _service.Update(character.ID, request);
        var dbFetch = Characters.Include(c => c.Connection).First(c => c.ID == character.ID);
        Assert.Equal(dbFetch.Connection.ID, connection.ID);
    }

    [Fact]
    public void Update_InvalidConnectionThrows(){
        var character = CharacterFactory.Create(Db());
        var request = new CharacterDTO.Update{ConnectionID = _faker.Random.Guid()};
        Assert.Throws<KeyNotFoundException>(() => _service.Update(character.ID, request));
    }

    [Fact]
    public void Update_InvalidIDReturnNull(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<CharacterDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_Successfull(){
        var character = CharacterFactory.Create(Db());
        _service.Delete(character.ID);
        Assert.Null(Characters.Find(character.ID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_OnlyRequestedDeleted(int index){
        var characters = Enumerable.Range(0, 10).Select(_ => CharacterFactory.Create(Db())).ToList();
        _service.Delete(characters[index].ID);

        var expected = characters.Where(w => w.ID != characters[index].ID).ToList();
        var actual = Characters.Include(c => c.Connection).Where(w => w.ID != characters[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
    }
}
