// WorldServiceTests.cs
using Yggdrasil.Services;
using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Util;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Data;
using Yggdrasil.Extensions;

namespace Yggdrasil.Tests.Services;

public class WorldServiceTests : DatabaseTestBase
{
    private readonly WorldService _service;
    private readonly Faker _faker = new();

    public WorldServiceTests(DatabaseFixture fixture) : base(fixture){
        _service = new WorldService(fixture.CreateContext());
    }
    // Helpers

    private List<World> CreateWorlds(int count = 1){
        return Enumerable.Range(0, count).Select(t => WorldFactory.Create(_fixture)).ToList();
    }

    private List<Character> GetCharactersFromDB(Guid World_ID, AppDbContext? context = null){
        context ??= _fixture.CreateContext();
        return context.Set<World>().Include(w => w.Characters).First(w => w.ID == World_ID).Characters;
    }

    // Tests
    
    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        CreateWorlds(count);
        var fetched = _service.GetAll().Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        CreateWorlds(100);

        var fetched = _service.GetAll(count).Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_NoErrorOnOverCount(){
        int count = 3;
        CreateWorlds(count);
        var fetched = _service.GetAll(5).Data!;
        Assert.Equal(count, fetched.Count);
    }
    
    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){   
        CreateWorlds(3);
        var fetch = _service.GetAll();

        Assert.IsType<ServiceResult<List<WorldSummary>>>(fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows(int count){
        CreateWorlds(3);
        Assert.Throws<ArgumentException>(() => _service.GetAll(count));
    }

    [Fact]
    public void GetAll_EmptyReturnsEmpty(){
        var world = _service.GetAll().Data!;

        Assert.Empty(world);
    }

    [Fact]
    public void GetLastUsed_Success(){
        var world = WorldFactory.Create(_fixture);
        var fetch = _service.GetLastUsed().Data!;
        Assert.Equivalent(world, fetch);
    }

    [Fact]
    public void GetLastUsed_GetCorrectFromMany(){
        var worlds = CreateWorlds(3);
        var lastUsed = worlds.OrderByDescending(w => w.LastUsed).FirstOrDefault();
        var fetch = _service.GetLastUsed().Data!;
        Assert.Equivalent(lastUsed, fetch);
    }

    [Fact]
    public void GetLastUsed_ReturnsCorrectServiceResultType(){
        CreateWorlds(1);
        var fetch = _service.GetLastUsed();
        Assert.IsType<ServiceResult<World>>(fetch);
    }

    [Fact]
    public void GetOne_GetRequested(){
        var world = WorldFactory.Create(_fixture);
        var fetch = _service.GetOne(world.ID).Data!;
        Assert.Equivalent(world, fetch);

        var fetched = _fixture.CreateContext().Set<World>().Include(w => w.Characters).FirstOrDefault(w => w.ID == world.ID);
        Assert.Equivalent(world, fetched);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void GetOne_GetCorrectFromMany(int index){
        var worlds = Enumerable.Range(0, 2).Select(w => WorldFactory.Create(_fixture)).ToList();

        var worldToGet = worlds[index];
        
        var fetch = _service.GetOne(worldToGet.ID).Data!;
        Assert.Equivalent(worldToGet, fetch);

        foreach(var other in worlds.Where(w => w != worldToGet)){
            Assert.Throws<EquivalentException>(() => Assert.Equivalent(other, fetch));
        }
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var world = WorldFactory.Create(_fixture);
        var fetch = _service.GetOne(world.ID);
        Assert.IsType<ServiceResult<World>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.GetOne(_faker.Random.Guid()));
    }

    [Fact]
    public void Creates_Success(){
        var request = AutoFaker.Generate<WorldRequest>();
        var convertedRequest = request.ConvertModelToDTO<World>();
        var world = _service.Create(request).Data!;
        var ID = world.ID;
        world.ID = Guid.Empty;

        Assert.Equivalent(convertedRequest, world, strict: false);

        world.ID = ID;
        // Check DB
        var fetched = _fixture.CreateContext().Set<World>().FirstOrDefault(w => w.ID == ID);
        Assert.Equivalent(fetched, world);
    }

    [Fact]
    public void Creates_DefaultInstruction(){
        var request = AutoFaker.Generate<WorldRequest>();
        request.NarratorInstruction = null;
        var result = _service.Create(request).Data!;

        Assert.Equal(Yggdrasil.Models.Settings.NARRATION_PROMPT, result.NarratorInstruction);
    }

    [Fact]
    public void Creates_CorrectReturnType(){
        var result = _service.Create(AutoFaker.Generate<WorldRequest>());

        Assert.IsType<ServiceResult<World>>(result);
    }

    [Fact]
    public void Deletes_DeletesTheWorld(){
        var context = _fixture.CreateContext();
        var world_ID = WorldFactory.Create(context).ID;

        _service.Delete(world_ID);

        // Verify deleted in DB
        var dbFetch = context.Set<World>().FirstOrDefault(w => w.ID == world_ID);
        Assert.Null(dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Delete_OnlyRequestedDeleted(int index){
        var worlds = Enumerable.Range(0, 3).Select(w => WorldFactory.Create(_fixture)).ToList();
        var worldToDelete = worlds[index];

        _service.Delete(worldToDelete.ID);

        foreach(var world in worlds){
            var dbFetch = _fixture.CreateContext().Set<World>().FirstOrDefault(w => w.ID == world.ID);

            if(world == worldToDelete)
                Assert.Null(dbFetch);
            else
                Assert.Equivalent(world, dbFetch);
        }
    }

    [Fact]
    public void Deletes_OnlyDeletedWorldRemovedFromCharacter(){
        var worlds = Enumerable.Range(0, 2).Select(_ => WorldFactory.Create(_fixture)).ToList();
        var character = CharacterFactory.Create(_fixture);

        var context = _fixture.CreateContext();
        worlds.ForEach(world =>
        {
            context.Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID).Characters.Add(context.Set<Character>().First(c => c.ID == character.ID));
        });
        context.SaveChanges();

        _service.Delete(worlds[0].ID);

        var dbCharacter = _fixture.CreateContext().Set<Character>().Include(c => c.Worlds).FirstOrDefault(c => c.ID == character.ID);
        Assert.Contains(dbCharacter.Worlds, w => w.ID == worlds[1].ID);
        Assert.DoesNotContain(dbCharacter.Worlds, w => w.ID == worlds[0].ID);
    }

    [Fact]
    public void Delete_RemovesWorldReference(){
        var world_ID = WorldFactory.Create(_fixture).ID;
        var character = CharacterFactory.Create(_fixture, world_ID);

        var dbWorld = _fixture.CreateContext().Set<World>().Include(w => w.Characters).FirstOrDefault(w => w.ID == world_ID)!;
        Assert.Contains(dbWorld.Characters, c => c.ID == character.ID);

        _service.Delete(world_ID);

        var dbCharacter = _fixture.CreateContext().Set<Character>().Include(c => c.Worlds).FirstOrDefault(c => c.Worlds.Any(w => w.ID == world_ID));
        Assert.Null(dbCharacter);
    }

    [Fact]
    public void Delete_CorrectReturnType(){
        var world_ID = WorldFactory.Create(_fixture).ID;

        var ret = _service.Delete(world_ID);

        Assert.IsType<ServiceResult<Empty>>(ret);

        Assert.Equal(ret.StatusCode, ServiceResult<Empty>.NoContent().StatusCode);
    }

    [Fact]
    public void Delete_InvalidGuidThrows(){
        CreateWorlds(1);
        Assert.Throws<KeyNotFoundException>(() => _service.Delete(_faker.Random.Guid()));
    }

    [Fact]
    public void AddCharacter_CharacterIsAdded(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        Assert.Empty(GetCharactersFromDB(world.ID));
        _service.AddCharacter(world.ID, character.ID);

        // Verify in DB:
        Assert.NotEmpty(GetCharactersFromDB(world.ID));
    }

    [Fact]
    public void AddCharacter_DuplicateNotSaved(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        Enumerable.Range(0, 2).Select(_ => _service.AddCharacter(world.ID, character.ID)).ToList();
        var characters = GetCharactersFromDB(world.ID);

        Assert.Single(characters);
    }

    [Fact]
    public void AddCharacter_WorldInCharacter(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        _service.AddCharacter(world.ID, character.ID);

        var dbCharacter = _fixture.CreateContext().Set<Character>().Include(c => c.Worlds).FirstOrDefault(c => c.ID == character.ID)!;
        Assert.Contains(dbCharacter.Worlds, w => w.ID == world.ID);
    }

    [Fact]
    public void AddCharacter_CorrectReturnType(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        var ret = _service.AddCharacter(world.ID, character.ID);
        Assert.IsType<ServiceResult<Character>>(ret);
    }

    [Fact]
    public void AddCharacter_InvalidWorldGuidThrows(){
        var character = CharacterFactory.Create(_fixture);
        Assert.Throws<KeyNotFoundException>(() => _service.AddCharacter(_faker.Random.Guid(), character.ID));
    }

    [Fact]
    public void AddCharacter_InvalidCharacterGuidThrows(){
        var world = WorldFactory.Create(_fixture);
        Assert.Throws<KeyNotFoundException>(() => _service.AddCharacter(world.ID, _faker.Random.Guid()));
    }

    [Fact]
    public void RemoveCharacter_CharacterIsRemoved(){
        var world = WorldFactory.Create(_fixture.CreateContext());
        var character = CharacterFactory.Create(_fixture.CreateContext(), world.ID);

        Assert.NotEmpty(GetCharactersFromDB(world.ID));
        _service.RemoveCharacter(world.ID, character.ID);
        Assert.Empty(GetCharactersFromDB(world.ID));
    }

    [Fact]
    public void RemoveCharacter_OnlyRequestedRemoved(){
        var context = _fixture.CreateContext();
        var world = WorldFactory.Create(context);
        int count = 3;

        var characters = Enumerable.Range(0, count).Select(i => CharacterFactory.Create(context, world.ID)).ToList();
        Character characterToRemove = characters[_faker.Random.Int(0, count - 1)];

        Assert.Equal(GetCharactersFromDB(world.ID).Count, count);

        _service.RemoveCharacter(world.ID, characterToRemove.ID);

        var dbCharacters = GetCharactersFromDB(world.ID);
        Assert.Equal((count - 1), dbCharacters.Count);
    }

    [Fact]
    public void RemoveCharacter_WorldRemovedFromCharacter(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        var context = _fixture.CreateContext();
        context.Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID).Characters.Add(context.Set<Character>().First(c => c.ID == character.ID));
        context.SaveChanges();

        _service.RemoveCharacter(world.ID, character.ID);

        var dbCharacter = _fixture.CreateContext().Set<Character>().Include(c => c.Worlds).FirstOrDefault(c => c.ID == character.ID);
        Assert.DoesNotContain(dbCharacter.Worlds, w => w.ID == world.ID);
    }



    [Fact]
    public void RemoveCharacter_RemovedFromRequestedWorldOnly(){
        var worlds = Enumerable.Range(0, 2).Select(_ => WorldFactory.Create(_fixture)).ToList();
        var character = CharacterFactory.Create(_fixture);

        var context = _fixture.CreateContext();
        worlds.ForEach(world =>
        {
            var w = context.Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID);
            var c = context.Set<Character>().First(c => c.ID == character.ID);
            w.Characters.Add(c);
        });
        context.SaveChanges();

        _service.RemoveCharacter(worlds[0].ID, character.ID);

        var dbCharacter = _fixture.CreateContext().Set<Character>().Include(c => c.Worlds).FirstOrDefault(c => c.ID == character.ID);
        Assert.Contains(dbCharacter.Worlds, w => w.ID == worlds[1].ID);
        Assert.DoesNotContain(dbCharacter.Worlds, w => w.ID == worlds[0].ID);
    }

    [Fact]
    public void RemoveCharacter_CorrectReturnOnSuccess(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture);

        var context = _fixture.CreateContext();
        context.Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID).Characters.Add(context.Set<Character>().First(c => c.ID == character.ID));
        context.SaveChanges();

        var ret = _service.RemoveCharacter(world.ID, character.ID);
        Assert.IsType<ServiceResult<Empty>>(ret);
        Assert.Equal(ret.StatusCode, ServiceResult<Empty>.NoContent().StatusCode);
    }

    [Fact]
    public void RemoveCharacter_InvalidWorldGuidThrows(){
        var context = _fixture.CreateContext();
        var world = WorldFactory.Create(context);
        var character = CharacterFactory.Create(context, world.ID);
        Assert.Throws<KeyNotFoundException>(() => _service.RemoveCharacter(_faker.Random.Guid(), character.ID));
    }

    [Fact]
    public void RemoveCharacter_InvalidCharacterGuidThrows(){
        var world = WorldFactory.Create(_fixture);
        Assert.Throws<KeyNotFoundException>(() => _service.RemoveCharacter(world.ID, _faker.Random.Guid()));
    }
}
