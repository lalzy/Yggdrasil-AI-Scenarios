// SystempromptService.cs

using System.Reflection;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Services;

public class SystemPromptServices(AppDbContext db){
    private readonly DbSet<SystemPrompt> _systemPrompt = db.Set<SystemPrompt>();
    
    /// <summary>Create and store a SystemPrompt to the Database</summary>
    /// <param name="request">A SystemPromptDTO Request</param>
    /// <returns>The saved SystemPrompt</returns>
    public SystemPrompt Create(SystemPromptDTO.Request request){
        var entity = new SystemPrompt { Name = request.Name};
        entity.Prompts = BasePrompts.Create();
        
        _systemPrompt.Add(entity);
        db.SaveChanges();
        return entity;
    }
    
    /// <summary>Fetch a requested SystemPrompt</summary>
    /// <param name="ID">The Guid of the systemPrompt</param>
    /// <returns>System Prompt if found, otherwise null</returns>
    /// <remarks>Returns owned prompts list</remarks>
    public SystemPrompt? Get(Guid ID){
        return _systemPrompt.FirstOrDefault(sp => sp.ID == ID);
    }

    /// <summary>Fetch all System prompts</summary>
    /// <param name="count">How many to fetch per page. Null or default returns all</param>
    /// <param name="pageIndex">Page index to fetch from</param>
    /// <param name="sortOrder">Sorting order.</param>
    /// <throws cref="ArgumentOutofrangeexception">Thrown if passing an invalid sortOrder</throws>
    public List<SystemPrompt> GetAll(int? count=null, int page=1, string sortOrder=SortOrder.IDAsc){
        IQueryable<SystemPrompt> querry = _systemPrompt.AsNoTracking().OrderBy(sortOrder);

        if(count != null){
            querry = querry.Skip(--page * count.Value).Take(count.Value);
        }
        return querry.ToList();
    }

    /// <summary>Renames the SystemPrompt's identifying name</summary>
    /// <param name="ID">The SystemPrompt ID to edit</param>
    /// <param name="name">The new name</param>
    /// <returns>The changed SystemPrompt</returns>
    public SystemPrompt Rename(Guid ID, string name){
        var entity = _systemPrompt.Find(ID)!;
        entity.Name = name;
        db.SaveChanges();
        return entity;
    }

    /// <summary>Delete System prompt</summary>
    /// <param name="ID">The SystemPrompt ID to delete</param>
    /// <throws cref="InvalidOperationException">Thrown if attemptign to delete DefaultPrompt</throws>
    public void Delete(Guid ID){
        var settings = db.Set<Settings>().Include(s => s.DefaultPrompt).Include(s => s.ActivePrompt).First();
        if(ID == settings.DefaultPrompt.ID) throw new InvalidOperationException("Cannot delete the default system prompt");
        else if(settings.ActivePrompt.ID == ID) settings.ActivePrompt = settings.DefaultPrompt;
        
        var entity = _systemPrompt.FirstOrDefault(p => p.ID == ID);
        if(entity == null) return;
        
        _systemPrompt.Remove(entity);
        
        db.SaveChanges();
    }

    /// <summary>Add a prompt to the systemPrompt</summary>
    /// <param name="ID">SystemPrompt ID to add to</param>
    /// <param name="prompt">The prompt object to add</param>
    /// <returns>The adjusted SystemPrompt</returns>
    /// <throws cref="ArgumentNullException">Thrown if prompt is null</throws>
    /// <throws cref="ArgumentException">Thrown if prompt.Name already exist (case-Insensitive)</throws>
    public SystemPrompt AddPrompt(Guid ID, Prompt prompt){
        var entity = _systemPrompt.Find(ID)!;
        
        if(prompt == null) throw new ArgumentNullException(nameof(prompt));
        
        else if(entity.Prompts.Any(sp => string.Equals(sp.Name, prompt.Name, StringComparison.CurrentCultureIgnoreCase))) throw new ArgumentException($"A prompt named {prompt.Name} already exists.");

        entity.Prompts.Add(prompt);
        db.SaveChanges();
        return entity;
    }
    
    /// <summary>Replace the prompts with passed prompts</summary>
    /// <param name="ID">SystemPrompt ID to add to</param>
    /// <param name="prompts">The List of prompts</param>
    /// <returns>The adjusted SystemPrompt</returns>
    public SystemPrompt? ReplacePrompts(Guid ID, List<Prompt> prompts){
        // Must have the base Prompts in the listing.
        if(!BasePrompts.Create().All(b => prompts.Any(p => p.Name == b.Name))) throw new ArgumentException("BasePrompts were removed");

        var systemPrompt = _systemPrompt.Find(ID);

        systemPrompt!.Prompts = prompts;
        db.SaveChanges();
        return systemPrompt;
    }
}
