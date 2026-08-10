// PersonaServiceTests.cs

using Yggdrasil.Tests.Factories;
using Yggdrasil.Services;
using Yggdrasil.Models;
using Yggdrasil.DTO; 
using Yggdrasil.Util;
using Yggdrasil.Extensions;

namespace Yggdrasil.Tests.Services;

public class PersonaTests : DatabaseTestBase{
    private readonly PersonaService _service;
    private readonly Faker _faker = new();

    public PersonaTests(DatabaseFixture fixture) :base(fixture){
        _service = new PersonaService(fixture.CreateContext());
    }

    // Helpers
    private void CreatePersonas(int count = 1){
        var context = _fixture.CreateContext();
        for (int i = 0; i < count; i++){
            PersonaFactory.Create(context);
        }
    }

    private Persona? GetPersonaFromDB(Guid persona_ID){
        return _fixture.CreateContext().Set<Persona>().FirstOrDefault(p => p.ID == persona_ID);
    }

    // Tests
    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        CreatePersonas(count);

        var fetch = _service.GetAll().Data!;

        Assert.Equal(count, fetch.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        CreatePersonas(100);

        var fetch = _service.GetAll(count).Data!;
        Assert.Equal(count, fetch.Count);
    }

    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){
        CreatePersonas(3);
        var fetch = _service.GetAll();

        Assert.IsType<ServiceResult<List<PersonaSummary>>>(fetch);
    }

    [Fact]
    public void GetAll_EmptyReturnsEmtpy(){
        var fetch = _service.GetAll().Data!;

        Assert.Empty(fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows(int count){
        CreatePersonas(3);
        Assert.Throws<ArgumentException>(() => _service.GetAll(count));
    }

    [Fact]
    public void GetOne_GetRequested(){
        var persona = PersonaFactory.Create(_fixture);
        var fetch = _service.GetOne(persona.ID).Data!;

        Assert.Equivalent(persona, fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetOne_GetCorrectFromMany(int index){
        var personas = Enumerable.Range(0, 3).Select(p => PersonaFactory.Create(_fixture)).ToList();
        var characterToGet = personas[index];
        var fetch = _service.GetOne(characterToGet.ID).Data!;

        Assert.Equivalent(characterToGet, fetch);

        foreach(var other in personas.Where(p => p != characterToGet)){
            Assert.Throws<EquivalentException>(() => Assert.Equivalent(other, fetch));
        }
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var persona = PersonaFactory.Create(_fixture);

        var fetch = _service.GetOne(persona.ID);

        Assert.IsType<ServiceResult<Persona>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        CreatePersonas(2);
        Assert.Throws<KeyNotFoundException>(() => _service.GetOne(_faker.Random.Uuid()));
    }

    [Fact]
    public void Create_Success(){
        var request = AutoFaker.Generate<PersonaRequest>();
        var convertedRequest = request.ConvertModelToDTO<Persona>();
        var persona = _service.Create(request).Data!;
        var ID = persona.ID;
        persona.ID = Guid.Empty; // Request doesn't have ID
        Assert.Equivalent(convertedRequest, persona, strict: false);
        
        persona.ID = ID; // Add Id back for DB check
        var dbFetched = _fixture.CreateContext().Set<Persona>().FirstOrDefault(p => p.ID == ID);
        Assert.Equivalent(persona, dbFetched);
    }

    [Fact]
    public void Create_ReturnType(){
        var request = AutoFaker.Generate<PersonaRequest>();
        var ret = _service.Create(request);

        Assert.IsType<ServiceResult<Persona>>(ret);
    }

    [Fact]
    public void Delete_DeletesThePersona(){
        Persona persona = PersonaFactory.Create(_fixture);
        Persona dbPersona = GetPersonaFromDB(persona.ID)!;

        Assert.Equivalent(persona, dbPersona);

        _service.Delete(persona.ID);
        dbPersona = GetPersonaFromDB(persona.ID);
        Assert.Null(dbPersona);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Delete_OnlyRequestedDeleted(int index){
        var personas = Enumerable.Range(0, 3).Select(p => PersonaFactory.Create(_fixture)).ToList();
        var personaToDelete = personas[index];

        _service.Delete(personaToDelete.ID);

        foreach(var persona in personas){
            var dbFetch = _fixture.CreateContext().Set<Persona>().FirstOrDefault(p => p.ID == persona.ID);
            if (persona == personaToDelete)
                Assert.Null(dbFetch);
            else
                Assert.Equivalent(persona, dbFetch);
        }
    }

    [Fact]
    public void Delete_CorrectReturnType(){
        Persona persona = PersonaFactory.Create(_fixture);

        var ret = _service.Delete(persona.ID);
        Assert.IsType<ServiceResult<Empty>>(ret);
    }

    [Fact]
    public void Delete_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.Delete(_faker.Random.Guid()));
    }
}
