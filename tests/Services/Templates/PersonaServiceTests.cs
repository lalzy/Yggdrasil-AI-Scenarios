// PersonaServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services.Templates;

public class PersonaServiceTests : DatabaseSetup{
    private readonly PersonaService _service;
    private readonly Faker _faker = new();
    private DbSet<PersonaTemplate> Personas => Db().Set<PersonaTemplate>();

    public PersonaServiceTests(){
        _service = new PersonaService(Db());
    }
}
