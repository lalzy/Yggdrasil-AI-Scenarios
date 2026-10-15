// AppDbContext.cs

using System.Reflection;
using Yggdrasil.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata;

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

        // Set up Cascade (for all, including nullable)
        foreach(var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Cascade;

        // Make Prompt in System Prompts a Json
        modelBuilder.Entity<SystemPrompt>().OwnsMany(sp => sp.Prompts, p => p.ToJson());

        base.OnModelCreating(modelBuilder);
    }

    private void UpdateUpdatedAt(EntityEntry entry, DateTime now, bool isChat){
        if(entry.Metadata.FindProperty("UpdatedAt") != null){
            entry.Property("UpdatedAt").CurrentValue = now;
        }

        foreach(var owner in GetOwners(entry)){
            if(owner.Entity is WorldInstance){
                if(isChat) owner.Property(nameof(WorldInstance.LastUsed)).CurrentValue = now;
            }else{
                UpdateUpdatedAt(owner, now, isChat);
            }
        }
    }

    
    private IEnumerable<EntityEntry> GetOwners(EntityEntry entry){
        foreach(var foreignKey in entry.Metadata.GetForeignKeys().Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade)){
            var keys = foreignKey.Properties
                            .Select(p => entry.Property(p.Name).CurrentValue).ToList();

            var owner = ChangeTracker.Entries().FirstOrDefault(e => foreignKey.PrincipalEntityType.IsAssignableFrom(e.Metadata) &&
                                                               foreignKey.PrincipalKey.Properties.Select(p => e.Property(p.Name).CurrentValue).SequenceEqual(keys));

            if(owner != null)
                yield return owner;
        }
    }

    private int SaveWithUniqueGuid(){
        // Esnure unique Guid for ID
        while(true){
            try{
                return base.SaveChanges();
            }catch(DbUpdateException exception){
                var added = exception.Entries.Where(e => e.State == EntityState.Added).ToList();
                if(added.Count == 0 || exception.InnerException is not SqliteException {SqliteExtendedErrorCode: 1555}) throw;

                foreach(var entry in added){
                    var entity = entry.Entity;
                    entry.State = EntityState.Detached;
                    Entry(entity).Property("ID").CurrentValue = Guid.NewGuid();
                    Add(entity);

                }
            }
        }
    }

    public override int SaveChanges(){
        var now = _time.GetUtcNow().UtcDateTime;

        foreach(var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || (e.Entity is Chat && e.State == EntityState.Added)).ToList()){
            UpdateUpdatedAt(entry, now, entry.Entity is Chat);
        }

        return SaveWithUniqueGuid();
    }
}
