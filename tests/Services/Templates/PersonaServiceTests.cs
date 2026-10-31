// PersonaServiceTests.cs

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

public class PersonaServiceTests : DatabaseSetup{
    private readonly PersonaService _service;
    private readonly Faker _faker = new();
    private DbSet<PersonaTemplate> Personas => Db().Set<PersonaTemplate>();

    public PersonaServiceTests(){
        _service = new PersonaService(Db());
    }

    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesPersona(){
        var request = AutoFaker.Generate<PersonaDTO.Request>();
        
        var fetch = _service.Create(request);

        Assert.EquivalentWithExclusions(request, fetch, "ConnectionID", "Connection");

        var dbFetch = Personas.Find(fetch!.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "ConnectionID", "Connection");
    }
    
    // =======================================
    //                  GET              
    // =======================================
    
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var personas = Enumerable.Range(0, 10).Select(_ => PersonaFactory.Create(Db())).ToList();
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
    public void GetAll_GetAllPersonas(){
        var personas = Enumerable.Range(0, 100).Select(_ => PersonaFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(personas, fetch, strict:true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var personas = Enumerable.Range(0, 100).Select(_ => PersonaFactory.Create(Db())).ToList();
        var expectedIDs = personas.Select(w => w.ID).ToHashSet();
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
        Enumerable.Range(0, 100).Select(_ => PersonaFactory.Create(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
        var fetch = _service.GetAll(count: count, page:page);

        Assert.Equal(expected, fetch.Select(w => w.ID));
    }

    [Theory]
    [InlineData(SortOrder.IDAsc)]
    [InlineData(SortOrder.IDDesc)]
    [InlineData(SortOrder.NameAsc)]
    [InlineData(SortOrder.NameDesc)]
    [InlineData(SortOrder.ModifiedAsc)]
    [InlineData(SortOrder.ModifiedDesc)]
    [InlineData(SortOrder.CreatedAsc)]
    [InlineData(SortOrder.CreatedDesc)]
    public void GetAll_SortOrderReturned(string order){
        var personas = Enumerable.Range(0, 100).Select(_ => PersonaFactory.Create(Db())).ToList();
        var expected = personas.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);
        
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults(){
        var personas = Enumerable.Range(0, 20).Select(_ => PersonaFactory.Create(Db())).ToList();
        var expected = personas.AsQueryable().OrderBy(SortOrder.IDAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();
        
        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var persona = PersonaFactory.Create(Db());
        var request = EnsureUnique.Request<PersonaDTO.Update, PersonaTemplate>(persona);
        var fetch = _service.Update(persona.ID, request);
        
        Assert.Equivalent(request, fetch, strict:true);

        var dbFetch = Personas.Find(persona.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, strict:true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var personas = Enumerable.Range(0, 10).Select(_ => PersonaFactory.Create(Db())).ToList();
        var request = EnsureUnique.Request<PersonaDTO.Update, PersonaTemplate>(personas[index]);

        _service.Update(personas[index].ID, request);
        var expected = personas.Where(w => w.ID != personas[index].ID).ToList();
        var actual = Personas.Where(w => w.ID != personas[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var persona = PersonaFactory.Create(Db());
        var fullRequest = EnsureUnique.Request<PersonaDTO.Update, PersonaTemplate>(persona);
        var properties = fullRequest.GetType().GetProperties().ToArray();
        foreach(var property in properties){
            var before = properties.ToDictionary(
                p => p.Name,
                p => typeof(PersonaTemplate).GetProperty(p.Name)!.GetValue(Personas.Find(persona.ID))
            );

            _service.Update(persona.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

            var dbFetch = Personas.Find(persona.ID);
            foreach(var p in properties){
                var actual = typeof(PersonaTemplate).GetProperty(p.Name)!.GetValue(dbFetch);
                var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
                Assert.Equivalent(expected, actual);
            }
        }
    }

    [Fact]
    public void Update_OnlyPassedNestedFieldsChanged(string nestedProperty){
        var persona = PersonaFactory.Create(Db());
        Nesting.AssertOnlyPassedFieldsChanged(
            persona,
            () => Personas.Find(persona.ID),
            (PersonaDTO.Update r) => _service.Update(persona.ID, r),
            r => r.Profile!,
            c => c!.Profile,
            d => new PersonaDTO.Update{ Profile = d });
    }

    [Fact]
    public void Update_InvalidIDReturnNull(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<PersonaDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_Successfull(){
        var persona = PersonaFactory.Create(Db());
        _service.Delete(persona.ID);
        Assert.Null(Personas.Find(persona.ID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_OnlyRequestedDeleted(int index){
        var personas = Enumerable.Range(0, 10).Select(_ => PersonaFactory.Create(Db())).ToList();
        _service.Delete(personas[index].ID);

        var expected = personas.Where(w => w.ID != personas[index].ID).ToList();
        var actual = Personas.Where(w => w.ID != personas[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
    }
}
