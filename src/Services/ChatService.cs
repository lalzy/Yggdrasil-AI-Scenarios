// ChatService.cs

using Yggdrasil.DTO;
using Yggdrasil.Data;
using Yggdrasil.Extensions;
using Yggdrasil.Models;
using Yggdrasil.Constants;

namespace Yggdrasil.Services;

public class ChatService (AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public ServiceResult<List<ChatMessageSummary>> GetAll(Guid conversation_ID, int? amount=null){
        var query = _db.Set<ChatMessage>().Where(c => c.Conversation_ID == conversation_ID).OrderByDescending(c => c.TimeStamp).Select(c=> new ChatMessageSummary(c.ID, c.Conversation_ID,  c.Role, c.Content, c.TimeStamp));

        if(amount != null){
            if(amount.Value < 1) throw new ArgumentException(ErrorMessages.LESSTHANONE);
            query = query.Take(amount.Value);
        }
        
        return new(query.ToList());
    }
}
