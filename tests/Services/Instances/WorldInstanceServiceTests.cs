// WorldInstanceServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Instances;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services.Instances;

public class WorldInstanceServiceTests : DatabaseSetup{
    private readonly WorldInstanceService _service;
    private readonly Faker _faker = new();
    private DbSet<WorldTemplate> World => Db().Set<WorldTemplate>();

    public WorldInstanceServiceTests(){
        _service = new WorldInstanceService(Db());
    }
    
}
