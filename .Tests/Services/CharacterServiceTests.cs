// CharacterServiceTests.cs

using Yggdrasil.Tests.Factories;
using Yggdrasil.Services;
using Yggdrasil.Models;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Extensions;

namespace Yggdrasil.Tests.Services;

public class CharacterServiceTests :DatabaseTestBase{
    private readonly CharacterService _service;
    private readonly Faker _faker = new ();

    public CharacterServiceTests(DatabaseFixture fixture) : base(fixture){
        _service = new CharacterService(fixture.CreateContext());
    }

    // Helpers
    private void CreateCharacters(int count = 1, Guid? world_ID = null){
        var context = _fixture.CreateContext();
        for(int i = 0; i < count; i++){
            CharacterFactory.Create(context, world_ID);
        }
    } 
    
    private Character? GetCharacterFromDB(Guid characet_ID){
        var context = _fixture.CreateContext();
        return context.Set<Character>().FirstOrDefault(c => c.ID == characet_ID);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        CreateCharacters(count);

        var fetch = _service.GetAll().Data!;

        Assert.Equal(count, fetch.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        CreateCharacters(100);
        var fetch = _service.GetAll(count).Data!;

        Assert.Equal(count, fetch.Count);
    }

    [Fact]
    public void GetAll_NoErrorOnOverCount(){
        int count = 3;
        CreateCharacters(count);
        var fetched = _service.GetAll(5).Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){
        CreateCharacters(3);
        var fetch = _service.GetAll();

        Assert.IsType<ServiceResult<List<CharacterSummary>>>(fetch);
    }

    [Fact]
    public void GetAll_EmptyReturnsEmpty(){
        var fetch = _service.GetAll().Data!;

        Assert.Empty(fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows(int count){
        CreateCharacters(3);
        Assert.Throws<ArgumentException>(()=>_service.GetAll(count));
    }

    [Fact]
    public void GetOne_GetRequested(){
        var character = CharacterFactory.Create(_fixture.CreateContext());
        var fetch = _service.GetOne(character.ID).Data!;

        Assert.Equivalent(character, fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void GetOne_GetCorrectFromMany(int index){
        var characters = Enumerable.Range(0, 2).Select(c => CharacterFactory.Create(_fixture)).ToList();
        var characterToGet = characters[index];
        
        var fetch = _service.GetOne(characterToGet.ID).Data!;

        Assert.Equivalent(characterToGet, fetch);
        foreach(var other in characters.Where(c => c != characterToGet)){
            Assert.Throws<EquivalentException>(() => Assert.Equivalent(other, fetch));
        }
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var character = CharacterFactory.Create(_fixture);

        var fetch = _service.GetOne(character.ID);

        Assert.IsType<ServiceResult<Character>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        CreateCharacters(2);
        Assert.Throws<KeyNotFoundException>(()=>_service.GetOne(_faker.Random.Uuid()));
    }

    [Fact]
    public void Create_Success(){
        var request = AutoFaker.Generate<CharacterRequest>();
        var convertedRequest = request.ConvertModelToDTO<Character>();
        var character = _service.Create(request).Data!;
        var ID = character.ID;
        character.ID = Guid.Empty; // Request doesn't have ID.
        Assert.Equivalent(convertedRequest, character, strict:false);

        character.ID = ID; // add ID back for DB Check
        // Check for DB:
        var fetched = _fixture.CreateContext().Set<Character>().FirstOrDefault(c=>c.ID == ID);
        Assert.Equivalent(character, fetched);
    }

    [Fact]
    public void Create_ReturnType(){
        var request = AutoFaker.Generate<CharacterRequest>();
        var ret = _service.Create(request);

        Assert.IsType<ServiceResult<Character>>(ret);
    }

    [Fact]
    public void Delete_DeletesTheCharacter(){
        Character character = CharacterFactory.Create(_fixture);
        Character dbCharacter = GetCharacterFromDB(character.ID);

        Assert.Equivalent(
            character,
            dbCharacter
        );

        _service.Delete(character.ID);
        dbCharacter = GetCharacterFromDB(character.ID);
        Assert.Null(dbCharacter);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Delete_OnlyRequestedDeleted(int index){
        var characters = Enumerable.Range(0, 3).Select(p => CharacterFactory.Create(_fixture)).ToList();
        var personaToDelete = characters[index];

        _service.Delete(personaToDelete.ID);

        foreach(var character in characters){
            var dbFetch = _fixture.CreateContext().Set<Character>().FirstOrDefault(c => c.ID == character.ID);
            if (character == personaToDelete)
                Assert.Null(dbFetch);
            else
                Assert.Equivalent(character, dbFetch);
        }
    }

    [Fact]
    public void Delete_OnlyDeletedCharacterRemovedFromWorld(){
        var characters = Enumerable.Range(0, 2).Select(_ => CharacterFactory.Create(_fixture)).ToList();
        var world = WorldFactory.Create(_fixture);
        var context = _fixture.CreateContext();
        characters.ForEach(character =>{ context.Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID)
                                            .Characters.Add(context.Set<Character>().First(c => c.ID == character.ID));
        });
        
        context.SaveChanges();
        _service.Delete(characters[0].ID);
        var dbWorld = _fixture.CreateContext().Set<World>().Include(w => w.Characters).FirstOrDefault(w => w.ID == world.ID);
        Assert.Contains(dbWorld.Characters, c => c.ID == characters[1].ID);
        Assert.DoesNotContain(dbWorld.Characters, c => c.ID == characters[0].ID);
    }

    [Fact]
    public void Delete_CorrectReturnType(){
        Character character = CharacterFactory.Create(_fixture);

        var ret = _service.Delete(character.ID);
        Assert.IsType<ServiceResult<Empty>>(ret);
    }

    [Fact]
    public void Delete_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.Delete(_faker.Random.Guid()));
    }
}
