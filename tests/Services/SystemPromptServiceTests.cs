// SystemPromptServiceTests.cs

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
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using System.Linq.Dynamic.Core;
using System.Collections;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Tests.Services;

public class SystemPromptServicesTests : DatabaseSetup{
    private readonly SystemPromptServices _service;
    private readonly Faker _faker = new();

    public SystemPromptServicesTests(){
        _service = new SystemPromptServices(Db());
    }
}
