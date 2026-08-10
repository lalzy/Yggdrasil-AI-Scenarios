// ConversationService.cs

using Yggdrasil.Data;

namespace Yggdrasil.Services;

public class ConversationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;
}
