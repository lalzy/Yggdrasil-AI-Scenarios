// PartialRequest.cs

using System.Linq;
using System.Reflection;

namespace Yggdrasil.Tests.TestUtil;

public static class PartialRequest{
    /// <summary>Creates a new isntance of <typeparamref name="T"/> where only <paramref name="property"/>is copied from <paramref name="source"/>. All other properties are either null or the record's default.</summary>
    /// <param name="source">The object to copy the value from.</param>
    /// <param name="property">The single property to copy.</param>
    /// <returns>A new <typeparamref name="T"/> with one field set.</returns>
    public static T IsolatedFieldFromDTORecord<T>(T source, PropertyInfo property){
        var constructor = typeof(T).GetConstructors().First();
        var request = (T)constructor.Invoke(constructor.GetParameters()
                                              .Select(p => p.HasDefaultValue ? p.DefaultValue:null)
                                              .ToArray());
        property.SetValue(request, property.GetValue(source));
        return request;
    }
}
