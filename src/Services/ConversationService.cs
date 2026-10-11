// ConversationService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Services;

public class ConversationService (AppDbContext db){
    private readonly DbSet<Conversation> _conversation = db.Set<Conversation>();

}
