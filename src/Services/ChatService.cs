// ChatService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Mapster;

namespace Yggdrasil.Services;

public class ChatService (AppDbContext db){
    private readonly DbSet<Chat> _chat = db.Set<Chat>();
    private readonly DbSet<Conversation> _conversation = db.Set<Conversation>();
    
    /// <summary>Create a new chat message</summary>
    /// <param name="conversationID">The conversation ID that owns this chat message</param>
    /// <param name="request">The chatDTO request</param>
    /// <throws cref="KeyNotfoundexception">When conversation is not found</throws>
    /// <returns>The created chat object</returns>
    public Chat Create(Guid conversationID, ChatDTO.Request request){
        var conversation = _conversation.Find(conversationID);
        if(conversation == null) throw new KeyNotFoundException("Conversation not found");
        
        var entity = request.Adapt<Chat>();
        entity.Conversation = conversation;
        _chat.Add(entity);
        db.SaveChanges();
        return entity;
    }

    /// <summary>Get requested chat message</summary>
    /// <param name="ID">ID of chatMessage to get</param>
    /// <returns>The requested chat object</returns>
    public Chat Get(Guid ID){
        return _chat.Include(c => c.Conversation).ThenInclude(c => c.World).FirstOrDefault(c => c.ID == ID)!;
    }

    /// <summary>Change content of requested Chat Message</summary>
    /// <param name="ID">ID of message to change</param>
    /// <param name="content">The new message</param>
    /// <returns>The updated chat object</returns>
    public Chat Update(Guid ID, string content){
        var chat = _chat.Find(ID)!;
        
        chat.Content = content;
        db.SaveChanges();
        
        return chat;
    }

    
    /// <summary>Delete chat message</summary>
    /// <param name="ID">ID of message to delete</param>
    public void Delete(Guid ID){
        var chat = _chat.Find(ID);
        if(chat == null) return;
        
        _chat.Remove(chat);
        db.SaveChanges();
    }
}
