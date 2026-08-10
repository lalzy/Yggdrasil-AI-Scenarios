// ServiceResult.cs

namespace Yggdrasil.Extensions;

///<summary>Empty type for noContent</summary>
public struct Empty {}
public record ServiceResult<T>(T? Data, string? Error = null, int StatusCode = 200){
    public bool Success => Error == null;

    public static ServiceResult<T> Ok(T data) => new(data);
    public static ServiceResult<T> NoContent() => new(default, StatusCode: 204); 
    public static ServiceResult<T> Created(T data) => new(data, StatusCode: 201);
    public static ServiceResult<T> NotFound(string error) => new(default, error, 404);
    public static ServiceResult<T> BadRequest(string error) => new(default, error, 400);
    public static ServiceResult<T> Conflict(string error) => new(default, error, 409);
    public static ServiceResult<T> InternalError(string error) => new(default, error, 500);
}
