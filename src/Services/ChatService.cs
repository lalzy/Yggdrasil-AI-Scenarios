// ChatService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Services;

public class ChatService (AppDbContext db){
    private readonly DbSet<Chat> _conversation = db.Set<Chat>();
}
