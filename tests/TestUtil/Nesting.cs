// Nesting.cs
using System.Text.Json;
using Xunit;


namespace Yggdrasil.Tests.TestUtil;


public static class Nesting{
    /// <summary>
    /// Asserts that updating one field of a nested object changes only that field.
    /// For each property of <typeparamref name="TDto"/>, sends an update with only that
    /// field set, then checks that field has the new value and all other fields are unchanged.
    /// </summary>
    /// <typeparam name="TUpdate">The update request type.</typeparam>
    /// <typeparam name="TEntity">The stored entity type.</typeparam>
    /// <typeparam name="TDto">The nested partial update DTO type.</typeparam>
    /// <typeparam name="TNested">The nested object type on the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <param name="find">Loads the entity from the database.</param>
    /// <param name="update">Sends the update request to the service.</param>
    /// <param name="getDto">Gets the nested DTO from an update request.</param>
    /// <param name="getNested">Gets the nested object from the entity.</param>
    /// <param name="wrap">Wraps a nested DTO in an update request.</param>
    public static void AssertOnlyPassedFieldsChanged<TUpdate, TEntity, TDto, TNested>(
        TEntity entity,
        Func<TEntity> find,
        Action<TUpdate> update,
        Func<TUpdate, TDto> getDto,
        Func<TEntity, TNested> getNested,
        Func<TDto, TUpdate> wrap) where TUpdate : class {
        var full = getDto(EnsureUnique.Request<TUpdate, TEntity>(entity));
        var properties = typeof(TDto).GetProperties();
        foreach(var property in properties){
            var json = JsonSerializer.Serialize(getNested(find()));
            var before = JsonSerializer.Deserialize<TNested>(json)!;

            update(wrap(PartialRequest.IsolatedFieldFromDTORecord(full, property)));

            var db = getNested(find());
            foreach(var p in properties){
                var nestedProperty = typeof(TNested).GetProperty(p.Name)!;
                var actual = nestedProperty.GetValue(db);
                var expected = p.Name == property.Name ? property.GetValue(full) : nestedProperty.GetValue(before);
                Assert.Equivalent(expected, actual);
            }
        }
    }
}
