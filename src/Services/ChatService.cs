// ChatService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using Mapster;

namespace Yggdrasil.Services;

public class ChatService (AppDbContext db){
    private readonly DbSet<Chat> _chat = db.Set<Chat>();
    private readonly DbSet<Conversation> _conversation = db.Set<Conversation>();
    

    public Chat Create(Guid conversationID, ChatDTO.Request request){
        var conversation = _conversation.Find(conversationID);
        if(conversation == null) throw new KeyNotFoundException("Conversation not found");
        
        var entity = request.Adapt<Chat>();
        entity.Conversation = conversation;
        _chat.Add(entity);
        db.SaveChanges();
        return entity;
    }

    public Chat Get(Guid ID){
        var conv = new Conversation{Title = ""};
        var chat = new Chat { Conversation = conv, Content = "" };
        return chat;
    }
}
