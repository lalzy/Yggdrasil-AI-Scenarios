// MapsterExtensions.cs

using Mapster;

namespace Yggdrasil.Extensions;

/// <summary>Shared Mapster helpers</summary>
public static class MapsterExtensions{
    private static readonly TypeAdapterConfig PatchConfig = CreatePatchConfig();

    private static TypeAdapterConfig CreatePatchConfig(){
        var config = new TypeAdapterConfig();
        config.Default.IgnoreNullValues(true);
        return config;
    }

    /// <summary>Creates a new <typeparamref name="TDestination"/> from <paramref name="source"/> ignoring the null values and using destination's default</summary>
    /// <param name="source">Object holding the values to add to the new adapted object</param>
    /// <returns>The new adapted object</returns>
    public static TDestination Patch<TDestination>(this object source){
        return source.Adapt<TDestination>(PatchConfig);
    }
}
