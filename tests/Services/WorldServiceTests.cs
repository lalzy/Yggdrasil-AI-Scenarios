// WorldServiceTests.cs
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

public class WorldServiceTests : DatabaseSetup{
    private readonly WorldService _service;
    private readonly Faker _faker = new();
    private DbSet<World> World => Db().Set<World>();

    public WorldServiceTests(){
        _service = new WorldService(Db());
    }
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesWorld(){
        var request = AutoFaker.Generate<WorldDTO.Request>();
        var fetch = _service.Create(request);

        Assert.Equivalent(request, fetch);
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
