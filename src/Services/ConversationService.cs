// ConversationService.cs

using Yggdrasil.Extensions;
using Yggdrasil.Data;
using Yggdrasil.Models;
using Yggdrasil.DTO;
using Yggdrasil.Constants;
using Yggdrasil.Util;

namespace Yggdrasil.Services;

public class ConversationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    
    /// <summary>Get all conversations as summary</summary>
    /// <param name="world_ID">ID of the world to fetch conversations from</param>
    /// <param name="count">How many to fetch</param>
    /// <returns>List of conversationSummaries</returns>
    /// <exception cref="KeyNotFoundexception">Thrown when world not found</exception>
    /// <exception cref="Argumentexception">Thrown when count is less than one</exception>
    public ServiceResult<List<ConversationSummary>> GetAllFromWorld(Guid world_ID, int? count=null){
        if(_db.Set<World>().FirstOrDefault(w => w.ID == world_ID) == null) throw new KeyNotFoundException();

        var query = _db.Set<Conversation>().Where<Conversation>(c => c.World_ID == world_ID).Select(c=> new ConversationSummary(c.ID, c.Title, c.CreatedAt));

        if(count.HasValue){
            if (count < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(count.Value);
        }

        return new(query.ToList());
    }

    
    /// <summary>Get requested conversations</summary>
    /// <param name="world_ID">ID of the conversation  to fetch</param>
    /// <returns>Conversation object</returns>
    /// <exception cref="KeyNotFoundexception">Thrown when conversation not found</exception>
    public ServiceResult<Conversation> GetOne(Guid conversation_ID){
        var conversation = _db.Set<Conversation>().FirstOrDefault(c => c.ID == conversation_ID);
        if(conversation == null) throw new KeyNotFoundException(ErrorMessages.CONVERSATION_NOT_FOUND);
        return new(conversation);
    }

    ///<summary>Create a conversation</summary>
    ///<param name="request">Conversation Request</param>
    ///<returns>The created conversation</returns>
    ///<exception cref="KeyNotfoundexception">Thrown if world does not exist</exception>
    public ServiceResult<Conversation> Create(ConversationRequest request){
        if (_db.Set<World>().FirstOrDefault(w => w.ID == request.World_ID) == null) throw new KeyNotFoundException();
        Conversation conversation = request.ConvertModelToDTO<Conversation>();

        _db.Set<Conversation>().Add(conversation);
        _db.SaveChanges();

        return new(conversation);
    }

    ///<summary>Deleted conversation</summary>
    ///<param name="conversation_ID">The ID of conversation to delete</param>
    ///<returns>NoContent/Empty</returns>
    ///<exception cref="KeyNotfoundexception">Thrown if conversation does not exist</exception>
    public ServiceResult<Empty> Delete(Guid conversation_ID){
        var conversation = _db.Set<Conversation>().FirstOrDefault(c => c.ID == conversation_ID);

        if(conversation == null) throw new KeyNotFoundException(ErrorMessages.CONVERSATION_NOT_FOUND);

        _db.Set<Conversation>().Remove(conversation);
        _db.SaveChanges();

        return ServiceResult<Empty>.NoContent();
    }
}
