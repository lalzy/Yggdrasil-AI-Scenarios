// LLMConnectionServiceTests.cs

using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Services;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Services;

public class LLMConnectionServiceTests : DatabaseSetup{
    private readonly LLMConnectionService _service;
    private readonly Faker _faker = new();
    private DbSet<LLMConnection> World => Db().Set<LLMConnection>();

    public LLMConnectionServiceTests(){
        _service = new LLMConnectionService(Db());
    }
    
}
