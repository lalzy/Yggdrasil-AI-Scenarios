// EnsureUniques.cs

using AutoBogus;

namespace Yggdrasil.Tests.TestUtil;

public static class EnsureUnique{
    /// <summary> Generates a random <typeparamref name="TRequest"/> where no property value match property of <paramref name="entity"/></summary>
    /// <typeparam name="TRequest">The request type to generate.</typeparam>
    /// <typeparam name="TEntity">The entity type to compare against.</typeparam>
    /// <param name="entity">The entity whose property values must all differ.</param>
    /// <returns>A generated request with every property value different from <paramref name="entity"/>.</returns>
    public static TRequest Request<TRequest, TEntity>(TEntity entity) where TRequest : class{
        var pairs = typeof(TRequest).GetProperties()
            .Select(p => (Req: p, Ent: typeof(TEntity).GetProperty(p.Name)))
            .Where(x => x.Ent != null)
            .ToList();
        // var properties = typeof(TRequest).GetProperties();
        TRequest request;
        do{
            request = AutoFaker.Generate<TRequest>();
        }while(pairs.Any(x => Equals(x.Req.GetValue(request), x.Ent!.GetValue(entity))));
        // }while(properties.Any(p => Equals(p.GetValue(request), typeof(TEntity).GetProperty(p.Name)?.GetValue(entity))));
        return request;
    }
}
