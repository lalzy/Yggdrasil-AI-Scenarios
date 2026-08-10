// CharacterService.cs

using Yggdrasil.Constants;
using Yggdrasil.Extensions;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Models;


namespace Yggdrasil.Services;

public class CharacterService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    ///<summary>Get a summary list of all characters</summary>
    ///<param name="count">How many to fetch</param>
    ///<returns>Service result with a list of character summary objects (name, description)</return>
    ///<exception cref="ArgumentException">Count is under one</exception>
    public ServiceResult<List<CharacterSummary>> GetAll(int? count=null){
        var query = _db.Set<Character>().Where<Character>(c=>c.Name != null && c.Description != null && c.Gender != null).Select(c=> new CharacterSummary(c.ID, c.Name, c.Description, c.Gender));
        if (count.HasValue){
            if(count < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(count.Value);
        }

        return new (query.ToList());
    }

    ///<summary>Get a specific character</summary>
    ///<param name="character_ID">The ID of the character to get</param>
    ///<returns>Service result with the character object as data</return>
    ///<exception cref="KeyNotfoundexception">Character not found</exception>
    public ServiceResult<Character> GetOne(Guid character_ID){
        var character = _db.Set<Character>().FirstOrDefault(c=>c.ID == character_ID);
        if(character == null) throw new KeyNotFoundException(ErrorMessages.CHARACTER_NOT_EXIST);
        return new (character);
    }

    ///<summary>Create a character</summary>
    ///<param name="request">Filled out CharacterRequest object</param>
    ///<returns>ServiceResult containing character</return>
    public ServiceResult<Character> Create(CharacterRequest request){
        var character = request.ConvertModelToDTO<Character>();

        _db.Set<Character>().Add(character);
        _db.SaveChanges();
        return new (character);
    }

    ///<summary>Delete a character</summary>
    ///<param name="character_ID">ID of the character to delete</param>
    ///<returns>ServiceResult with no data</returns>
    ///<exception cref="KeyNotFOundexception">Character not found</exception>
    public ServiceResult<Empty> Delete(Guid character_ID){
        var rows = _db.Set<Character>().Where(c=>c.ID == character_ID).ExecuteDelete();
        if(rows == 0) throw new KeyNotFoundException(ErrorMessages.CHARACTER_NOT_EXIST);
        return ServiceResult<Empty>.NoContent();
    }
}
