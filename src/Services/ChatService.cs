// ChatService.cs

using Yggdrasil.DTO;
using Yggdrasil.Data;
using Yggdrasil.Extensions;
using Yggdrasil.Models;
using Yggdrasil.Constants;
using Yggdrasil.Util;

namespace Yggdrasil.Services;

public class ChatService (AppDbContext db)
{
    private readonly AppDbContext _db = db;

    /// <summary>Get all chat Messages from a conversation as a SummaryRecord.</summary>
    /// <param name="conversation_ID">The conversation ID</param>
    /// <param name="count">How many records to fetch</param>
    /// <returns>ServiceResult with data of list of chatSummaries</returns>
    /// <exception cref="Argumentexception">Count is less than one</exception>
    public ServiceResult<List<ChatMessageSummary>> GetAll(Guid conversation_ID, int? amount=null){
        var query = _db.Set<ChatMessage>().Where(c => c.Conversation_ID == conversation_ID).OrderByDescending(c => c.TimeStamp).Select(c=> new ChatMessageSummary(c.ID, c.Conversation_ID,  c.Role, c.Content, c.TimeStamp));

        if(amount != null){
            if(amount.Value < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(amount.Value);
        }
        
        return new(query.ToList());
    }

    /// <summary>Get the requested ChatMessage object</summary>
    /// <param name="message_ID">ID of the ChatMessage to fetch</param>
    /// <returns>ServiceResult with the ChatMessage as Data</returns>
    /// <exception cref="KeyNotFoundexception">Thrown when ChatMessage not found</exception>
    public ServiceResult<ChatMessage> GetOne(Guid message_ID){
        var message = _db.Set<ChatMessage>().FirstOrDefault(m => m.ID == message_ID);
        if(message == null) throw new KeyNotFoundException(ErrorMessages.CONVERSATION_NOT_FOUND);
        return new(message);
    }

    
    /// <summary> Create a new ChatMessage</summary>
    /// <param name="name">Name of the World</param>
    /// <param name="description">Description of the world</param>
    /// <param name="narratorInstructions">A custom instruction if desired</param>
    /// <returns>Service Result with Data as World Object</returns>
    /// <exception cref="KeyNotFoundException">Thrown when Conversation not found</exception>
    public ServiceResult<ChatMessage> Create(ChatMessageRequest request){
        if((_db.Set<Conversation>().FirstOrDefault(c => c.ID == request.Conversation_ID)) == null)
            throw new KeyNotFoundException(ErrorMessages.CONVERSATION_NOT_FOUND);
        
        ChatMessage message = request.ConvertModelToDTO<ChatMessage>();
        message = _db.Set<ChatMessage>().Add(message).Entity;
        _db.SaveChanges();
        return new(message);
    }


    /// <summary>Update the Content of ChatMessage</summary>
    /// <param name="message_ID">ID of the ChatMessage to Edit</param>
    /// <param name="request">The DTO of fields/data to update (only Content)</param>
    /// <returns>ServiceResult with the ChatMessage as Data</returns>
    /// <exception cref="NullReferenceException">Thrown when ChatMessage not found</exception>
    public ServiceResult<ChatMessage> Update(Guid message_ID, ChatMessageUpdateRequest request){
        var message = _db.Set<ChatMessage>().Find(message_ID);

        message.Content = request.Content ?? message.Content;
        if(request.Content != null) message.Content = request.Content;
        
        _db.SaveChanges();
        return new(message);
    }

    ///<summary>Delete message</summary>
    ///<param name="message_ID">ID of message to delete</param>
    ///<returns>No content</returns>
    ///<exception cref="KeyNotfoundexception">Thrown if chatMessage is not found</exception>
    public ServiceResult<Empty> Delete(Guid message_ID){
        var message = _db.Set<ChatMessage>().FirstOrDefault(m => m.ID == message_ID);
        if(message == null) throw new KeyNotFoundException(ErrorMessages.MESSAGE_NOT_FOUND);

        _db.Set<ChatMessage>().Remove(message);
        _db.SaveChanges();
        
        return ServiceResult<Empty>.NoContent();
    }
}
