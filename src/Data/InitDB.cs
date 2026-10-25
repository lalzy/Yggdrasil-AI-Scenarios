// InitDb.cs

using Yggdrasil.Models.Entities;

namespace Yggdrasil.Data;

public static class InitDB{
    public static void Initialize(this WebApplication app){
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureCreated();
    }
}
