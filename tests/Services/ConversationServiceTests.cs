// ConversationServiceTests.cs

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

public class ConversationServiceTests : DatabaseSetup{
    private readonly ConversationService _service;
    private readonly Faker _faker = new();
    private DbSet<Conversation> Conversation => Db().Set<Conversation>();

    public ConversationServiceTests(){
        _service = new ConversationService(Db());
    }

    // Helper to create a ConversationDTO with empty ChatList
    private ConversationDTO.Request CreateDTORequest(){
        return new ConversationDTO.Request(_faker.Lorem.Word());
    }

    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    // Single test because there really isn't anything else to test
    public void Create_CreatesConversation(){
        var request = CreateDTORequest();
        var fetch = _service.Create(request);
        var dbFetch = Conversation.FirstOrDefault(c => c.ID == fetch.ID);
        
        Assert.Equivalent(request, fetch);
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
        var conversations = Enumerable.Range(0, 10).Select(_ => ConversationFactory.Create(Db())).ToList();
        var fetch = _service.Get(conversations[index].ID);
        Assert.Equivalent(conversations[index], fetch);
    }

    [Fact]
    public void Get_InvalidIDReturnNull(){
        var fetch = _service.Get(_faker.Random.Guid());
        Assert.Null(fetch);
    }
    
    // =======================================
    //                 RENAME              
    // =======================================

    [Fact]
    public void Rename_SuccessfullyRename(){
        var conversation = ConversationFactory.Create(Db());
        string newTitle = "";
        do{
            newTitle = _faker.Lorem.Word();
        }while(newTitle == conversation.Title);

        var fetch = _service.Rename(conversation.ID, newTitle);
        var dbFetch = Conversation.Find(conversation.ID);

        Assert.Equal(newTitle, fetch.Title);
        Assert.Equal(newTitle, dbFetch.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Rename_OnlyRequestedIsChanged(int index){
        var conversations = Enumerable.Range(0, 10).Select(_ => ConversationFactory.Create(Db())).ToList();
        var newTitle = "";
        do{
            newTitle = _faker.Lorem.Word();
        }while(conversations.Any((c) => c.Title == newTitle));

        _service.Rename(conversations[index].ID, newTitle);

        var expected = conversations.Where(c => c.ID != conversations[index].ID).ToList();
        var actual = Conversation.Include(c => c.Chats).Where(c => c.ID != conversations[index].ID).ToList();

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Rename_ReturnNullOnInvalidID(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }
    
    // =======================================
    //                 DELETE              
    // =======================================

    [Fact]
    public void Delete_DeletesConversation(){
        var conversation = ConversationFactory.Create(Db());
        
        _service.Delete(conversation.ID);

        var dbFetch = Conversation.Find(conversation.ID);
        Assert.Null(dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_DeleteOnlyRequested(int index){
        var conversations =  Enumerable.Range(0, 10).Select(_ => ConversationFactory.Create(Db())).ToList();
        _service.Delete(conversations[index].ID);

        var actual = conversations.Where(c => c.ID != conversations[index].ID).ToList();
        var expected = conversations.ToList();

        Assert.Equivalent(actual, expected, strict:true);
    }

    [Fact]
    public void Delete_DoesNothingOnInvalidID(){
        var exceptions = Record.Exception(() => _service.Delete(_faker.Random.Guid()));
        Assert.Null(exceptions);
    }

    // =======================================
    //                DUPLICATE           
    // =======================================
    
}
