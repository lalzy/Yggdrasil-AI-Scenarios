// LLMConnectionServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Yggdrasil.Services;
using Yggdrasil.Tests.Factories;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Data;
using Yggdrasil.Models.Enums.SortOrders;
using System.Linq.Dynamic.Core;
using System.Collections;

namespace Yggdrasil.Tests.Services;

public class LLMConnectionServiceTests : DatabaseSetup{
    private readonly LLMConnectionService _service;
    private readonly Faker _faker = new();
    private DbSet<LLMConnection> LLMConnections => Db().Set<LLMConnection>();

    public LLMConnectionServiceTests(){
        _service = new LLMConnectionService(Db());
    }
    
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesLLMConnections(){
        var request = AutoFaker.Generate<LLMConnectionDTO.Request>();
        var fetch = _service.Create(request);

        Assert.Equivalent(request, fetch);

        var dbFetch = LLMConnections.Find(fetch.ID);
        Assert.Equivalent(request, dbFetch);
    }
    
    // =======================================
    //                  GET              
    // =======================================
    
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var LLMConnections = Enumerable.Range(0, 10).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var fetch = _service.Get(LLMConnections[index].ID);

        Assert.Equivalent(LLMConnections[index], fetch);
    }

    [Fact]
    public void Get_InvalidIDReturnNull(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }
    
    
    // =======================================
    //               GET ALL             
    // =======================================

    [Fact]
    public void GetAll_Success(){
        var LLMConnections = Enumerable.Range(0, 100).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(LLMConnections, fetch, strict:true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var LLMConnections = Enumerable.Range(0, 100).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var expectedIDs = LLMConnections.Select(w => w.ID).ToHashSet();
        var fetch = _service.GetAll(count:count);

        Assert.Equal(count, fetch.Count);
        Assert.All(fetch, w => Assert.Contains(w.ID, expectedIDs));
        
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 10)]
    [InlineData(3, 30)]
    [InlineData(9, 10)]
    public void GetAll_ReturnsRequestedPage(int page, int count){
        Enumerable.Range(0, 100).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
        var fetch = _service.GetAll(count: count, page:page);

        Assert.Equal(expected, fetch.Select(w => w.ID));
    }

    [Theory]
    [InlineData(LLMConnectionSortOrder.IDAsc)]
    [InlineData(LLMConnectionSortOrder.IDDesc)]
    [InlineData(LLMConnectionSortOrder.NameAsc)]
    [InlineData(LLMConnectionSortOrder.NameDesc)]
    [InlineData(LLMConnectionSortOrder.URLAsc)]
    [InlineData(LLMConnectionSortOrder.URLDesc)]
    [InlineData(LLMConnectionSortOrder.ModelAsc)]
    [InlineData(LLMConnectionSortOrder.ModelDesc)]
    [InlineData(LLMConnectionSortOrder.ModifiedAsc)]
    [InlineData(LLMConnectionSortOrder.ModifiedDesc)]
    [InlineData(LLMConnectionSortOrder.CreatedAsc)]
    [InlineData(LLMConnectionSortOrder.CreatedDesc)]
    public void GetAll_SortOrderReturned(string order){
        var LLMConnections = Enumerable.Range(0, 100).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var expected = LLMConnections.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);
        
        Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
    }

    [Fact]
    public void GetAll_UsesDefaults(){
        var LLMConnections = Enumerable.Range(0, 100).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var expected = LLMConnections.AsQueryable().OrderBy(LLMConnectionSortOrder.IDAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
        var actual = _service.GetAll().Select(w => w.ID).ToList();
        
        Assert.Equal(expected, actual);
    }
    
    // =======================================
    //                UPDATE              
    // =======================================

    
    [Fact]
    public void Update_SuccessfullyUpdates(){
        var LLMConnection = LLMConnectionFactory.Create(Db());
        var request = EnsureUnique.Request<LLMConnectionDTO.Update, LLMConnection>(LLMConnection);
        var fetch = _service.Update(LLMConnection.ID, request);
        
        Assert.Equivalent(request, fetch);

        var dbFetch = LLMConnections.Find(LLMConnection.ID);
        Assert.Equivalent(request, dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyChangesRequested(int index){
        var LLMConnections = Enumerable.Range(0, 10).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        var request = EnsureUnique.Request<LLMConnectionDTO.Update, LLMConnection>(LLMConnections[index]);
        _service.Update(LLMConnections[index].ID, request);
        var expected = LLMConnections.Where(w => w.ID != LLMConnections[index].ID).ToList();
        var actual = LLMConnections.Where(w => w.ID != LLMConnections[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Update_OnlyPassedFieldsChanged(){
        var LLMConnection = LLMConnectionFactory.Create(Db());
        var fullRequest = EnsureUnique.Request<LLMConnectionDTO.Update, LLMConnection>(LLMConnection);
        var properties = fullRequest.GetType().GetProperties();
        foreach(var property in properties){
            var before = properties.ToDictionary(
                p => p.Name,
                p => typeof(LLMConnection).GetProperty(p.Name)!.GetValue(LLMConnections.Find(LLMConnection.ID))
            );

            _service.Update(LLMConnection.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

            var dbFetch = LLMConnections.Find(LLMConnection.ID);
            foreach(var p in properties){
                var actual = typeof(LLMConnection).GetProperty(p.Name)!.GetValue(dbFetch);
                var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
                Assert.Equivalent(expected, actual);
            }
        }
    }

    [Fact]
    public void Update_InvalidIDReturnNull(){
        var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<LLMConnectionDTO.Update>());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_Successfull(){
        var LLMConnection = LLMConnectionFactory.Create(Db());
        _service.Delete(LLMConnection.ID);
        Assert.Null(LLMConnections.Find(LLMConnection.ID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_OnlyRequestedDeleted(int index){
        var LLMConnections = Enumerable.Range(0, 10).Select(_ => LLMConnectionFactory.Create(Db())).ToList();
        _service.Delete(LLMConnections[index].ID);

        var expected = LLMConnections.Where(w => w.ID != LLMConnections[index].ID).ToList();
        var actual = LLMConnections.Where(w => w.ID != LLMConnections[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict:true);
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
    }
}
