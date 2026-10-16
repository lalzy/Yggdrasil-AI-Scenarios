// WorldTemplateServiceTests.cs
using FluentAssertions;
using AutoBogus;
using Bogus;
using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Services;

public class WorldTemplateServiceTests : DatabaseSetup{
    private readonly WorldTemplateService _service;
    private readonly Faker _faker = new();
    private DbSet<WorldTemplate> World => Db().Set<WorldTemplate>();

    public WorldTemplateServiceTests(){
        _service = new WorldTemplateService(Db());
    }
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesWorld(){
        var request = AutoFaker.Generate<WorldDTO.Request>();
        var fetch = _service.Create(request);

        Assert.EquivalentWithExclusions(request, fetch, "OpeningMessage");
        Assert.True(fetch.OpeningMessages.Contains(request.OpeningMessage));

        var dbFetch = World.Find(fetch.ID);
        Assert.EquivalentWithExclusions(request, dbFetch, "OpeningMessage");
        Assert.True(dbFetch!.OpeningMessages.Contains(request.OpeningMessage));
    }

    [Fact]
    public void Create_DoesNotAddNullMessage(){
        var request = new AutoFaker<WorldDTO.Request>().RuleFor(x => x.OpeningMessage, _ => null).Generate();
        var fetch = _service.Create(request);
        Assert.Empty(fetch.OpeningMessages);
    }
    
    // =======================================
    //                  GET              
    // =======================================
    
    // =======================================
    //                  EDIT              
    // =======================================

    // =======================================
    //                 DELETE              
    // =======================================

}
