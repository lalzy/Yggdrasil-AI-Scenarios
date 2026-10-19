// NarratorServiceTests.cs

// using FluentAssertions;
// using AutoBogus;
// using Bogus;
// using System.Text.Json;
// using Yggdrasil.Tests.TestUtil;
// using Yggdrasil.Services;
// using Yggdrasil.Services.Templates;
// using Yggdrasil.Models.Entities;
// using Yggdrasil.Models.DTO;
// using Microsoft.EntityFrameworkCore; 
// using Yggdrasil.Tests.Factories;
// using Yggdrasil.Models.Enums.SortOrders;
// using Yggdrasil.Data;
// using System.Runtime.CompilerServices;
// using System.Linq.Dynamic.Core;
// using System.Collections;

// namespace Yggdrasil.Tests.Services.Templates;

// public class NarratorServiceTests : DatabaseSetup{
//     private readonly NarratorService _service;
//     private readonly Faker _faker = new();
//     private DbSet<Narrator> Narrators => Db().Set<Narrator>();

//     public NarratorServiceTests(){
//         _service = new NarratorService(Db());
//     }
//     // =======================================
//     //                 CREATE            
//     // =======================================

//     [Fact]
//     public void Create_CreatesNarrators(){
//         var request = AutoFaker.Generate<NarratorDTO.Request>();
//         var fetch = _service.Create(request);

//         Assert.Equivalent(request, fetch);

//         var dbFetch = Narrators.Find(fetch.ID);
//         Assert.Equivalent(request, dbFetch);
//     }
    
//     // =======================================
//     //                  GET              
//     // =======================================
    
//     [Theory]
//     [InlineData(0)]
//     [InlineData(3)]
//     [InlineData(5)]
//     public void Get_RetrieveRequested(int index){
//         var narrators = Enumerable.Range(0, 10).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var fetch = _service.Get(narrators[index].ID);

//         Assert.Equivalent(narrators[index], fetch);
//     }

//     [Fact]
//     public void Get_InvalidIDReturnNull(){
//         Assert.Null(_service.Get(_faker.Random.Guid()));
//     }
    
//     // =======================================
//     //               GET ALL             
//     // =======================================

//     [Fact]
//     public void GetAll_GetAllNarrators(){
//         var narrators = Enumerable.Range(0, 100).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var fetch = _service.GetAll();
//         Assert.Equivalent(narrators, fetch, strict:true);
//     }

//     [Theory]
//     [InlineData(1)]
//     [InlineData(30)]
//     [InlineData(50)]
//     public void GetAll_GetCountRequested(int count){
//         var narrators = Enumerable.Range(0, 100).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var expectedIDs = narrators.Select(w => w.ID).ToHashSet();
//         var fetch = _service.GetAll(count:count);

//         Assert.Equal(count, fetch.Count);
//         Assert.All(fetch, w => Assert.Contains(w.ID, expectedIDs));
        
//     }

//     [Theory]
//     [InlineData(1, 10)]
//     [InlineData(2, 10)]
//     [InlineData(3, 30)]
//     [InlineData(9, 10)]
//     public void GetAll_ReturnsRequestedPage(int page, int count){
//         Enumerable.Range(0, 100).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var all = _service.GetAll(count: 100);
//         var expected = all.Skip((page - 1) * count).Take(count).Select(w => w.ID);
//         var fetch = _service.GetAll(count: count, page:page);

//         Assert.Equal(expected, fetch.Select(w => w.ID));
//     }

//     [Theory]
//     [InlineData(NarratorSortOrder.IDAsc)]
//     [InlineData(NarratorSortOrder.IDDesc)]
//     [InlineData(NarratorSortOrder.NameAsc)]
//     [InlineData(NarratorSortOrder.NameDesc)]
//     [InlineData(NarratorSortOrder.DescriptionAsc)]
//     [InlineData(NarratorSortOrder.DescriptionDesc)]
//     [InlineData(NarratorSortOrder.ModifiedAsc)]
//     [InlineData(NarratorSortOrder.ModifiedDesc)]
//     [InlineData(NarratorSortOrder.CreatedAsc)]
//     [InlineData(NarratorSortOrder.CreatedDesc)]
//     public void GetAll_SortOrderReturned(string order){
//         var narrators = Enumerable.Range(0, 100).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var expected = narrators.AsQueryable().OrderBy(order, new CaseInsensitiveComparer());
//         var fetch = _service.GetAll(sortOrder: order);
        
//         Assert.Equal(expected.Select(w => w.ID), fetch.Select(w => w.ID));
//     }

//     [Fact]
//     public void GetAll_UsesDefaults(){
//         var narrators = Enumerable.Range(0, 20).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var expected = narrators.AsQueryable().OrderBy(NarratorSortOrder.NameAsc).ThenBy(w => w.ID).Select(w => w.ID).ToList();
//         var actual = _service.GetAll().Select(w => w.ID).ToList();
        
//         Assert.Equal(expected, actual);
//     }
    
//     // =======================================
//     //                UPDATE              
//     // =======================================

    
//     [Fact]
//     public void Update_SuccessfullyUpdates(){
//         var narrator = NarratorFactory.Create(Db());
//         var request = EnsureUnique.Request<NarratorDTO.Update, Narrator>(narrator);
//         var fetch = _service.Update(narrator.ID, request);
        
//         Assert.Equivalent(request, fetch);

//         var dbFetch = Narrators.Find(narrator.ID);
//         Assert.Equivalent(request, dbFetch);
//     }

//     [Theory]
//     [InlineData(0)]
//     [InlineData(3)]
//     [InlineData(5)]
//     public void Update_OnlyChangesRequested(int index){
//         var narrators = Enumerable.Range(0, 10).Select(_ => NarratorFactory.Create(Db())).ToList();
//         var request = EnsureUnique.Request<NarratorDTO.Update, Narrator>(narrators[index]);
//         _service.Update(narrators[index].ID, request);
//         var expected = narrators.Where(w => w.ID != narrators[index].ID).ToList();
//         var actual = Narrators.Where(w => w.ID != narrators[index].ID).ToList();

//         Assert.Equivalent(expected, actual, strict:true);
//     }

//     [Fact]
//     public void Update_OnlyPassedFieldsChanged(){
//         var narrator = NarratorFactory.Create(Db());
//         var fullRequest = EnsureUnique.Request<NarratorDTO.Update, Narrator>(narrator);
//         var properties = fullRequest.GetType().GetProperties();
//         foreach(var property in properties){
//             var before = properties.ToDictionary(
//                 p => p.Name,
//                 p => typeof(Narrator).GetProperty(p.Name)!.GetValue(Narrators.Find(narrator.ID))
//             );

//             _service.Update(narrator.ID, PartialRequest.IsolatedFieldFromDTORecord(fullRequest, property));

//             var dbFetch = Narrators.Find(narrator.ID);
//             foreach(var p in properties){
//                 var actual = typeof(Narrator).GetProperty(p.Name)!.GetValue(dbFetch);
//                 var expected = p.Name == property.Name ? property.GetValue(fullRequest) : before[p.Name];
//                 Assert.Equivalent(expected, actual);
//             }
//         }
//     }

//     [Fact]
//     public void Update_InvalidIDReturnNull(){
//         var fetch = _service.Update(_faker.Random.Guid(), AutoFaker.Generate<NarratorDTO.Update>());
//         Assert.Null(fetch);
//     }
    
//     // =======================================
//     //                 DELETE              
//     // =======================================
    
//     [Fact]
//     public void Delete_Successfull(){
//         var narrator = NarratorFactory.Create(Db());
//         _service.Delete(narrator.ID);
//         Assert.Null(Narrators.Find(narrator.ID));
//     }

//     [Theory]
//     [InlineData(0)]
//     [InlineData(3)]
//     [InlineData(5)]
//     public void Delete_OnlyRequestedDeleted(int index){
//         var narrators = Enumerable.Range(0, 10).Select(_ => NarratorFactory.Create(Db())).ToList();
//         _service.Delete(narrators[index].ID);

//         var expected = narrators.Where(w => w.ID != narrators[index].ID).ToList();
//         var actual = Narrators.Where(w => w.ID != narrators[index].ID).ToList();

//         Assert.Equivalent(expected, actual, strict:true);
//     }

//     [Fact]
//     public void Delete_InvalidIDDoesNotThrow(){
//         Assert.Null(Record.Exception(() => _service.Delete(_faker.Random.Guid())));
//     }
// }
