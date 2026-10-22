// UniversalErrors.cs

namespace Yggdrasil.Utils;

public static class UniversalErrors{
    /// <summary>Create error messages string for when an ID is not found in requested table.</summary>
    /// <param name="ID">The requested ID</param>
    /// <returns>Error message string</returns>
    public static string CreateKeyNotFoundString(string table, Guid ID){
        return $"{table} with ID '{ID}' was not found.";
    }
}
