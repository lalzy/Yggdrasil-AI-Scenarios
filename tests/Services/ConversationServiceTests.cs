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

    [Fact]
    public void Create_CreatesConversation(){
        var request = CreateDTORequest();
        var fetch = _service.Create(request);
        var dbFetch = Conversation.Include(c => c.Chats).FirstOrDefault(c => c.ID == fetch.ID);
        
        Assert.Equivalent(request, fetch);
        Assert.Equivalent(request, dbFetch);
    }
}
