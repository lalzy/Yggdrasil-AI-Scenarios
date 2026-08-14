// WorldService.cs

using Yggdrasil.Constants;
using Yggdrasil.Models;
using Yggdrasil.Util;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Extensions;

namespace Yggdrasil.Services;

public class WorldService(AppDbContext db){
    private readonly AppDbContext _db = db;

    /// <summary>Get all world as a SummaryRecord.</summary>
    /// <param name="count">How many records to fetch</param>
    /// <returns>ServiceResult with data of list of worldSummaries</returns>
    /// <exception cref="Argumentexception">Count is less than one</exception>
    public ServiceResult<List<WorldSummary>> GetAll(int? count=null){
        var query = _db.Set<World>().Where<World>(w=>w.Name != null).Select(w=> new WorldSummary(w.ID, w.Name, w.Description));
        if(count.HasValue){ 
            if(count < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE); 
            query = query.Take(count.Value);
        }
        return new(query.ToList());
    }

    /// <summary>Get the requested world object</summary>
    /// <param name="world_ID">ID of the world to fetch</param>
    /// <returns>ServiceResult with the world as Data</returns>
    /// <exception cref="KeyNotFoundexception">Thrown when world not found</exception>
    public ServiceResult<World> GetOne(Guid world_ID){
        var world = _db.Set<World>().Include(w=>w.Characters).Where(w=>w.ID == world_ID).FirstOrDefault();
        if (world == null) throw new KeyNotFoundException(ErrorMessages.WORLD_NOT_EXIST);
    
        return new ServiceResult<World>(world);
    }

    /// <summary> Create a new World</summary>
    /// <param name="name">Name of the World</param>
    /// <param name="description">Description of the world</param>
    /// <param name="narratorInstructions">A custom instruction if desired</param>
    /// <returns>Service Result with Data as World Object</returns>
    public ServiceResult<World> Create(WorldRequest request){
        World world = request.ConvertModelToDTO<World>();
   
        world.NarratorInstruction ??= _db.Set<Yggdrasil.Models.Settings>().First().DefaultPrompt;
    
        world = _db.Set<World>().Add(world).Entity;
        _db.SaveChanges();

        return new ServiceResult<World>(world);
    }

    /// <summary>Delete a world</summary>
    /// <param name="world_ID"></param>
    /// <returns>Service Result with no Data</returns>
    /// <exception cref="KeyNotFoundexception">Thrown if world not found</exception>
    public ServiceResult<Empty> Delete(Guid world_ID){
        var world = _db.Set<World>().Include(w=>w.Characters).FirstOrDefault(w=>w.ID == world_ID);
        if(world == null) throw new KeyNotFoundException(ErrorMessages.WORLD_NOT_EXIST);
        
        _db.Set<World>().Remove(world);
        _db.SaveChanges();

        return ServiceResult<Empty>.NoContent();
    }

    /// <summary>Add a character to the world.</summary>
    /// <param name="world_ID">ID of the world to add to</param>
    /// <param name="character_ID">ID of character to add to the world</param>
    /// <returns>The Character</returns>
    /// <exception cref="KeyNotFoundexception">Thrown if either world or character is not found</exception>
    public ServiceResult<Character> AddCharacter(Guid world_ID, Guid character_ID){
        var world = _db.Set<World>().Include(w=>w.Characters).Where(w => w.ID == world_ID).FirstOrDefault();
        if(world == null) throw new KeyNotFoundException(ErrorMessages.WORLD_NOT_EXIST);
        var character = _db.Set<Character>().Where(c=>c.ID == character_ID).FirstOrDefault();
        if(character == null) throw new KeyNotFoundException(ErrorMessages.CHARACTER_NOT_EXIST);
        world.Characters.Add(character);
        // character.World_IDs.Add(world_ID);

        _db.SaveChanges();
        return new ServiceResult<Character>(character);
    }

    /// <summary>Remove a character from the world</summary>
    /// <param name="world_ID"></param>
    /// <param name="character_ID"></param>
    /// <returns>Service Result with no Data</returns>
    /// <exception cref="KeyNotfoundexception">Thrown if either world or charcter is not found</exception>
    public ServiceResult<Empty> RemoveCharacter(Guid world_ID, Guid character_ID){
        var world = _db.Set<World>().Include(w=>w.Characters).FirstOrDefault(w=>w.ID == world_ID);
        if (world == null) throw new KeyNotFoundException(ErrorMessages.WORLD_NOT_EXIST);
        var character = world.Characters.FirstOrDefault(c=>c.ID == character_ID);
        if(character == null) throw new KeyNotFoundException(ErrorMessages.CHARACTER_NOT_EXIST);
        
        world.Characters.Remove(character);
        // character.World_IDs.Remove(world_ID);
        _db.SaveChanges();

        return ServiceResult<Empty>.NoContent();
    }
}
