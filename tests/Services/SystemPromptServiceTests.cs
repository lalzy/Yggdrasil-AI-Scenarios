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
using Yggdrasil.Models.Enums;
using Yggdrasil.Data;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core;
using System.Collections;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Tests.Services;

public class SystemPromptServicesTests : DatabaseSetup{
    private readonly SystemPromptServices _service;
    private readonly Faker _faker = new();
    private DbSet<SystemPrompt> SystemPrompts => Db().Set<SystemPrompt>();

    public SystemPromptServicesTests(){
        _service = new SystemPromptServices(Db());
    }
    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_ReturnsSavedSystemPrompt(){
        var request = AutoFaker.Generate<SystemPromptDTO.Request>();
        var result = _service.Create(request);
        result.Should().BeEquivalentTo(request, o => o.ExcludingMissingMembers());
    }

    [Fact]
    public void Create_SavesSystemPromptToDatabase(){
        var request = AutoFaker.Generate<SystemPromptDTO.Request>();
        var result = _service.Create(request);
        
        var saved = SystemPrompts.Single();
        Assert.Equivalent(result, saved, strict:true);
    }

    [Fact]
    public void Create_AddsBasePrompts(){
        var request = AutoFaker.Generate<SystemPromptDTO.Request>();
        var result = _service.Create(request);

        var saved = SystemPrompts.Single();
        var expected = BasePrompts.Create();

        Assert.Equal(
            expected.Select(p => (p.Name, p.Content, p.Source, p.Active)),
            saved.Prompts.Select(p => (p.Name, p.Content, p.Source, p.Active)));
    }
    
    // =======================================
    //                  GET              
    // =======================================

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var prompts = Enumerable.Range(0, 10).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var fetch = _service.Get(prompts[index].ID);

        Assert.Equivalent(prompts[index], fetch, strict:true);
    }
    
    [Fact]
    public void Get_GetNullIfNotFound(){
        var fetch = _service.Get(_faker.Random.Guid());
        Assert.Null(fetch);
    }

    [Fact]
    public void Get_GetsPromptsOrdered()
    {
        var prompts = new AutoFaker<Prompt>().Generate(10);
        var systemPrompt = SystemPromptFactory.Create(Db(), prompts);

        var fetch = _service.Get(systemPrompt.ID)!;

        Assert.Equal(prompts.Select(p => p.Name), fetch.Prompts.Select(p => p.Name));
    }

    
    // =======================================
    //               GET ALL             
    // =======================================

    [Fact]
    public void GetAll_GetAllSystemPrompts(){
        var prompts = Enumerable.Range(0, 100).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var fetch = _service.GetAll();
        Assert.Equivalent(prompts, fetch, strict: true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(50)]
    public void GetAll_GetCountRequested(int count){
        var prompts = Enumerable.Range(0, 100).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var expectedIDs = prompts.Select(p => p.ID).ToHashSet();
        var fetch = _service.GetAll(count: count);

        Assert.Equal(count, fetch.Count);
        Assert.All(fetch, p => Assert.Contains(p.ID, expectedIDs));
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 10)]
    [InlineData(3, 30)]
    [InlineData(9, 10)]
    public void GetAll_ReturnsRequestedPage(int page, int count){
        Enumerable.Range(0, 100).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var all = _service.GetAll(count: 100);
        var expected = all.Skip((page - 1) * count).Take(count).Select(p => p.ID);
        var fetch = _service.GetAll(count: count, page: page);

        Assert.Equal(expected, fetch.Select(p => p.ID));
    }

    [Theory]
    [InlineData(SortOrder.IDAsc)]
    [InlineData(SortOrder.IDDesc)]
    [InlineData(SortOrder.NameAsc)]
    [InlineData(SortOrder.NameDesc)]
    [InlineData(SortOrder.ModifiedAsc)]
    [InlineData(SortOrder.ModifiedDesc)]
    [InlineData(SortOrder.CreatedAsc)]
    [InlineData(SortOrder.CreatedDesc)]
    public void GetAll_SortOrderReturned(string order){
        var prompts = Enumerable.Range(0, 100).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var expected = prompts.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
        var fetch = _service.GetAll(sortOrder: order);

        Assert.Equal(expected.Select(p => p.ID), fetch.Select(p => p.ID));
    }

    [Fact]
    public void GetALl_UsesDefaults(){
        var prompts = Enumerable.Range(0, 20).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var expected = prompts.AsQueryable().OrderBy(SortOrder.IDAsc).ThenBy(p => p.ID).Select(p => p.ID).ToList();
        var actual = _service.GetAll().Select(p => p.ID).ToList();

        Assert.Equal(expected, actual);
    }

    // =======================================
    //                RENAME              
    // =======================================

    [Fact]
    public void Rename_NameChangeSuccess(){
        var original = SystemPromptFactory.Create(Db());
        string newName="";
        do{
            newName = _faker.Lorem.Word();
        }while(newName == original.Name);

        var fetched = _service.Rename(original.ID, newName);

        Assert.Equal(newName, fetched.Name);
        
        fetched = SystemPrompts.Single(sp => sp.ID == original.ID);

        Assert.Equal(newName, fetched.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Rename_OnlyRequestedRenamed(int index){
        var prompts = Enumerable.Range(0, 10).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        string newName = "newName";
        _service.Rename(prompts[index].ID, newName);

        var expected = prompts.Where(sp => sp.ID != prompts[index].ID).ToList();
        var actual = SystemPrompts.Where(sp => sp.ID != prompts[index].ID).ToList();
        Assert.Equivalent(expected, actual, strict: true);
    }

    [Fact]
    public void Rename_ThrowsNullReferenceOnNotFound(){
        Assert.Throws<NullReferenceException>(() => {_service.Rename(_faker.Random.Guid(), _faker.Lorem.Word());
        });
    }

    // =======================================
    //                 DELETE              
    // =======================================
    
    [Fact]
    public void Delete_DeletesEntry(){
        // Settings is a required table due to holding a SystemPrompts entry.
        SettingsFactory.Create(Db());
        var toDelete = SystemPromptFactory.Create(Db());
        _service.Delete(toDelete.ID);
        var fetched = SystemPrompts.Find(toDelete.ID);
        Assert.Null(fetched);
    }

    [Fact]
    public void Delete_SystemPromptNotDeleted(){
        var settings = SettingsFactory.Create(Db());
        var toDelete = SystemPromptFactory.Create(Db());

        _service.Delete(toDelete.ID);
        var fetched = SystemPrompts.Find(settings.DefaultPrompt.ID);
        Assert.NotNull(fetched);
    }

    [Fact]
    public void Delete_OnlyRequestedEntryDeleted(){
        // Settings is a required table due to holding a SystemPrompts entry.
        var settings = SettingsFactory.Create(Db());
        var prompts = Enumerable.Range(0, 10).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        var toDelete = prompts[_faker.Random.Int(0, prompts.Count - 1)];
        var expected = SystemPrompts.Select(p => p.ID).Where(ID => ID != toDelete.ID).OrderBy(ID => ID).ToList();

        _service.Delete(toDelete.ID);

        var fetched = SystemPrompts.Select(p => p.ID).OrderBy(ID => ID).ToList();
        Assert.Equal(expected, fetched);
    }

    [Fact]
    public void Delete_DefaultSystemPromptProtected(){
        var settings = SettingsFactory.Create(Db());
        Assert.Throws<InvalidOperationException>(() => _service.Delete(settings.DefaultPrompt.ID));
    }

    [Fact]
    public void Delete_InvalidIDDoesNotThrow(){
        SettingsFactory.Create(Db());
        var exceptions = Record.Exception(() => _service.Delete(_faker.Random.Guid()));
        Assert.Null(exceptions);
    }

    [Fact]
    public void Delete_DeletingActivePromptSetsDefaultPrompt(){
        var db = Db();
        var settings = SettingsFactory.Create(db);
        var active = SystemPromptFactory.Create(db);
        settings.ActivePrompt = active;
        db.SaveChanges();
        
        _service.Delete(active.ID);

        var fetched = Db().Set<Settings>().Include(s => s.ActivePrompt).Include(s => s.DefaultPrompt).Single();
        Assert.Equal(fetched.DefaultPrompt.ID, fetched.ActivePrompt.ID);
    }

    [Fact]
    public void AddPrompt_SuccessfullyAdd(){
        var systemPrompt = SystemPromptFactory.Create(Db(), []);
        var prompt = new AutoFaker<Prompt>().Generate();

        Assert.Empty(systemPrompt.Prompts);

        // Returns the object
        var response = _service.AddPrompt(systemPrompt.ID, prompt);
        var dbFetch = SystemPrompts.First(sp => sp.ID == systemPrompt.ID);
        Assert.Equivalent(response, dbFetch, strict: true);

        // Has the new Prompt
        Assert.Single(dbFetch.Prompts);
        Assert.Equivalent(prompt, dbFetch.Prompts[0], strict: true);
    }

    // =======================================
    //               ADD PROMPT            
    // =======================================
    [Fact]
    public void AddPrompt_DuplicateNameThrowsArgumentException(){
        var systemPrompt = SystemPromptFactory.Create(Db());
        var newPrompt = AutoFaker.Generate<Prompt>();
        newPrompt.Name = systemPrompt.Prompts[0].Name!.ToLower();

        Assert.Throws<ArgumentException>(() => _service.AddPrompt(systemPrompt.ID, newPrompt));
        
        newPrompt.Name = systemPrompt.Prompts[0].Name!.ToUpper();

        Assert.Throws<ArgumentException>(() => _service.AddPrompt(systemPrompt.ID, newPrompt));
    }

    [Fact]
    public void AddPrompt_NewIsAppendedToLast()
    {
        var prompts = new AutoFaker<Prompt>().Generate(10);
        var systemPrompt = SystemPromptFactory.Create(Db(), prompts);

        var newPrompt = new AutoFaker<Prompt>().Generate();
        _service.AddPrompt(systemPrompt.ID, newPrompt);

        var fetch = SystemPrompts.First(sp => sp.ID == systemPrompt.ID);
        Assert.Equal(11, fetch.Prompts.Count);
        Assert.Equivalent(newPrompt, fetch.Prompts.Last(), strict: true);
    }
    
    [Fact]
    public void AddPrompt_NullPromptThrowsArgumentNullException(){
        var systemPrompt = SystemPromptFactory.Create(Db());
        Assert.Throws<ArgumentNullException>(() => _service.AddPrompt(systemPrompt.ID, null!));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void AddPrompt_OnlyRequestedChanged(int index){
        var prompts = Enumerable.Range(0, 10).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        _service.AddPrompt(prompts[index].ID, AutoFaker.Generate<Prompt>());

        var expected = prompts.Where(sp => sp.ID != prompts[index].ID).ToList();
        var actual = SystemPrompts.Where(sp => sp.ID != prompts[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict: true);
    }

    [Fact]
    public void AddPrompt_InvalidSystemPromptThrowsNullReferenceException(){
        var prompt = new Prompt();
        Assert.Throws<NullReferenceException>(() => _service.AddPrompt(_faker.Random.Guid(), prompt));
    }

    private List<Prompt> CreatePlausiblePrompts(int count){
        var prompts = BasePrompts.Create();
        prompts.AddRange(Enumerable.Range(0, count).Select(_ => AutoFaker.Generate<Prompt>()).ToList());
        return prompts;
    }

    // =======================================
    //            REPLACE PROMPTS          
    // =======================================
    [Fact]
    public void ReplacePrompts_ChangesEntirePromptListing(){
        var prompts = CreatePlausiblePrompts(10);

        var systemPrompt = SystemPromptFactory.Create(Db(), prompts);

        var newPrompts = CreatePlausiblePrompts(10);

        // Verify the database's prompts are the same
        var dbFetch = SystemPrompts.Find(systemPrompt.ID)!;
        Assert.Equivalent(dbFetch.Prompts, prompts, strict:true);

        // Verify the database was changed
        var fetched = _service.ReplacePrompts(systemPrompt.ID, newPrompts);
        dbFetch = SystemPrompts.Find(systemPrompt.ID)!;
        Assert.Equivalent(fetched.Prompts, newPrompts, strict:true);
        Assert.Equivalent(dbFetch.Prompts, newPrompts, strict:true);
    }

    [Fact]
    public void ReplacePrompts_EnsureMissingBasePromptThrows(){
        var prompts = Enumerable.Range(0, 10).Select(_ => AutoFaker.Generate<Prompt>()).ToList();
        var systemPrompt = SystemPromptFactory.Create(Db());

        Assert.Throws<ArgumentException>(() => _service.ReplacePrompts(systemPrompt.ID, prompts));
    }

    [Fact]
    public void ReplacePrompts_OrderIsRespected(){
        var prompts = CreatePlausiblePrompts(3);
        var systemPrompt = SystemPromptFactory.Create(Db(), prompts);

        prompts.Insert(0, AutoFaker.Generate<Prompt>());

        var fetched = _service.ReplacePrompts(systemPrompt.ID, prompts);
        var dbFetch = SystemPrompts.Find(systemPrompt.ID);

        Assert.Equal(JsonSerializer.Serialize(prompts), JsonSerializer.Serialize(fetched.Prompts));
        Assert.Equal(JsonSerializer.Serialize(prompts), JsonSerializer.Serialize(dbFetch.Prompts));
    }

    [Fact]
    public void ReplacePrompts_InvalidID(){
        var prompts = CreatePlausiblePrompts(1);
        Assert.Throws<NullReferenceException>(() => _service.ReplacePrompts(_faker.Random.Guid(), prompts));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void ReplacePrompts_OnlyUpdateRequestedPrompt(int index){
        var prompts = CreatePlausiblePrompts(1);
        var systemPrompts = Enumerable.Range(0, 10).Select(_ => SystemPromptFactory.Create(Db())).ToList();
        _service.ReplacePrompts(systemPrompts[index].ID, prompts);

        var expected = systemPrompts.Where(sp => sp.ID != systemPrompts[index].ID).ToList();
        var actual = SystemPrompts.Where(sp => sp.ID != systemPrompts[index].ID).ToList();

        Assert.Equivalent(expected, actual, strict: true);
    }
}
