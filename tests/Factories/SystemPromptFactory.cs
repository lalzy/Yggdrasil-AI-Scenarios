// SystemPromptServiceFactory.cs

using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using AutoBogus;
using Yggdrasil.Data;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Tests.Factories;

public static class SystemPromptFactory
{
    public static SystemPrompt Create(AppDbContext db, List<Prompt>? prompt = null)
    {
        prompt ??= [new AutoFaker<Prompt>().Generate()];

        var entity = new AutoFaker<SystemPrompt>()
            .RuleFor(x => x.ID, _ => Guid.Empty)
            .RuleFor(x => x.Prompts, _ => prompt)
            .Generate();

        db.Set<SystemPrompt>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}

