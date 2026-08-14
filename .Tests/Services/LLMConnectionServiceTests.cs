// LLMConnectionServiceTests

using Yggdrasil.Services;
using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Util;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Data;
using Yggdrasil.Extensions;

namespace Yggdrasil.Tests.Services;

public class LLMConnectionServiceTests : DatabaseTestBase{
    private readonly LLMConnectionService _service;
    private readonly Faker _faker = new();

    public LLMConnectionServiceTests(DatabaseFixture fixture) : base(fixture){
        _service = new LLMConnectionService(fixture.CreateContext());
    }

    private List<LLMConnection> CreateConnections(int amount=1){
        return Enumerable.Range(0,amount).Select(t=>LLMConnectionFactory.Create(_fixture)).ToList();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        CreateConnections(count);
        var fetched = _service.GetAll().Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        CreateConnections(100);

        var fetched = _service.GetAll(count).Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_NoErrorOnOverCount(){
        int count = 3;
        CreateConnections(count);
        var fetched = _service.GetAll(5).Data!;
        Assert.Equal(count, fetched.Count);
    }
    
    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){   
        CreateConnections(3);
        var fetch = _service.GetAll();

        Assert.IsType<ServiceResult<List<LLMConnectionSummary>>>(fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows(int count){
        CreateConnections(3);
        Assert.Throws<ArgumentException>(() => _service.GetAll(count));
    }

    [Fact]
    public void GetAll_EmptyReturnsEmpty(){
        var world = _service.GetAll().Data!;

        Assert.Empty(world);
    }

    [Fact]
    public void GetOne_GetRequested(){
        var connection = LLMConnectionFactory.Create(_fixture);
        var fetch = _service.GetOne(connection.ID).Data!;
        Assert.Equivalent(connection, fetch);

        var fetched = _fixture.CreateContext().Set<LLMConnection>().FirstOrDefault(c => c.ID == connection.ID);
        Assert.Equivalent(connection, fetched);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void GetOne_GetCorrectFromMany(int index){
        var connections = Enumerable.Range(0, 2).Select(w => LLMConnectionFactory.Create(_fixture)).ToList();

        var connectionToGet = connections[index];
        
        var fetch = _service.GetOne(connectionToGet.ID).Data!;
        Assert.Equivalent(connectionToGet, fetch);

        foreach(var other in connections.Where(w => w != connectionToGet)){
            Assert.Throws<EquivalentException>(() => Assert.Equivalent(other, fetch));
        }
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var connection = LLMConnectionFactory.Create(_fixture);
        var fetch = _service.GetOne(connection.ID);
        Assert.IsType<ServiceResult<LLMConnection>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.GetOne(_faker.Random.Guid()));
    }
}
