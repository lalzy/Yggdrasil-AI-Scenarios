// AppDbContext.cs

using System.Reflection;
using Yggdrasil.Models.Entities;

namespace Yggdrasil.Data;

public class AppDbContext : DbContext{
    private readonly TimeProvider _time;
    public AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider? time = null) : base(options) {
        _time = time ?? TimeProvider.System;
    }

    ///<summary>Register all Models as DB Tables automatically</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        var entityTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "Yggdrasil.Models.Entities" && t != typeof(Prompt));
        foreach(var type in entityTypes){
            modelBuilder.Entity(type);
        }
        modelBuilder.Entity<SystemPrompt>().OwnsMany(sp => sp.Prompts, p => p.ToJson());

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(){
        var now = _time.GetUtcNow().UtcDateTime;

        foreach(var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)){
            if(entry.Metadata.FindProperty("UpdatedAt") != null)
                entry.Property("UpdatedAt").CurrentValue = now;
        }

        return base.SaveChanges();
    }
}
