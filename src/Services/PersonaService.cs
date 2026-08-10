// PersonaService.cs

using Yggdrasil.Constants;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Extensions;
using Yggdrasil.Models;

namespace Yggdrasil.Services;

public class PersonaService(AppDbContext db){
    private readonly AppDbContext _db = db;

    ///<summary>Get a summary list of all personas</summary>
    ///<param name="count">How many to fetch</param>
    ///<returns>Service result with a list of acharacter summary objects (name, description)</returns>
    public ServiceResult<List<PersonaSummary>> GetAll(int? count=null){
        var query = _db.Set<Persona>().Where<Persona>(p => p.Name != null && p.Description != null && p.Gender != null).Select(p => new PersonaSummary(p.ID, p.Name, p.Description, p.Gender));
        if(count.HasValue){
            if(count.Value < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(count.Value);
        }
        return new(query.ToList());
    }

    
    ///<summary>Get a specific persona</summary>
    ///<param name="character_ID">The ID of the persona  to get</param>
    ///<returns>Service result with the persona  object as data</return>
    ///<exception cref="KeyNotfoundexception">Persona not found</exception>
    public ServiceResult<Persona> GetOne(Guid persona_ID){
        var persona = _db.Set<Persona>().FirstOrDefault(p => p.ID == persona_ID);
        if(persona == null) throw new KeyNotFoundException(ErrorMessages.PERSONA_NOT_FOUND);
        return new(persona);
    }
    
    ///<summary>Create a persona</summary>
    ///<param name="request">Filled out PersonaRequest object</param>
    ///<returns>ServiceResult containing persona</return>
    public ServiceResult<Persona> Create(PersonaRequest request){
        var persona = request.ConvertModelToDTO<Persona>();
        _db.Set<Persona>().Add(persona);
        _db.SaveChanges();
        return new(persona);
    }
    
    ///<summary>Delete a persona</summary>
    ///<param name="character_ID">ID of the persona to delete</param>
    ///<returns>ServiceResult with no data</returns>
    ///<exception cref="KeyNotFOundexception">persona not found</exception>
    public ServiceResult<Empty> Delete(Guid persona_ID){
        var rows = _db.Set<Persona>().Where(p => p.ID == persona_ID).ExecuteDelete();
        if(rows == 0) throw new KeyNotFoundException(ErrorMessages.PERSONA_NOT_FOUND);

        return ServiceResult<Empty>.NoContent();
    }
}
