// ObjectMerger.cs

using System.Text.Json;

namespace Yggdrasil.Util;

public class ObjectMerger
{
    ///<summary>Merges two objects to a singular object for use with RestAPI payloads</summary>
    ///<param name="objects">Objects to merge</param>
    ///<returns>new object as dictionary</returns>
    public static Dictionary<string, JsonElement> Merge(List<object> objects, JsonSerializerOptions? options = null){
        options ??= new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var result = new Dictionary<string, JsonElement>();
        var documents = objects.Select(o => JsonDocument.Parse(JsonSerializer.Serialize(o, options))).ToList();
        foreach(var document in documents){
            foreach(var p in document.RootElement.EnumerateObject()){
                result[p.Name] = p.Value;
            }
        }
        return result;
    }
}
