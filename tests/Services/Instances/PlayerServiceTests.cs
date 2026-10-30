// PlayerServiceTests.cs

using AutoBogus;
using Microsoft.EntityFrameworkCore;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Instances;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Tests.Factories;

namespace Yggdrasil.Tests.Services.Instances;

public class PlayerServiceTests : DatabaseSetup{
    private readonly PlayerService _service;
    private readonly Faker _faker = new();
    private DbSet<PersonaTemplate> Templates => Db().Set<PersonaTemplate>();
    private DbSet<PlayerInstance> Players => Db().Set<PlayerInstance>();

    public PlayerServiceTests(){
        _service = new PlayerService(Db());
    }
}
