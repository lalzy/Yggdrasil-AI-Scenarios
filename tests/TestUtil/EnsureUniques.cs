// EnsureUniques.cs

using System.Linq;
using AutoBogus;

namespace Yggdrasil.Tests.TestUtil;

public static class EnsureUnique{
    /// <summary> Generates a random <typeparamref name="TRequest"/> where no property value match property of <paramref name="entity"/></summary>
    /// <typeparam name="TRequest">The request type to generate.</typeparam>
    /// <typeparam name="TEntity">The entity type to compare against.</typeparam>
    /// <param name="entity">The entity whose property values must all differ.</param>
    /// <returns>A generated request with every property value different from <paramref name="entity"/>.</returns>
    public static TRequest Request<TRequest, TEntity>(TEntity entity) where TRequest : class{
        var properties = typeof(TRequest).GetProperties();
        TRequest request;
        do{
            request = AutoFaker.Generate<TRequest>();
        }while(properties.Any(p => Equals(p.GetValue(request), typeof(TEntity).GetProperty(p.Name)?.GetValue(entity))));
        return request;
    }
}
