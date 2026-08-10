// ServiceResultExtensions.cs

namespace Yggdrasil.Extensions;

public static class ServiceResultExtensions{
    ///<summary>Wrapper for controllers to get normalized HTTP payloads</summary>
    ///<returns>HTTP Response</returns>
    public static IActionResult ToResponse<T>(this ServiceResult<T> result){
        if (result.Success)
            return new ObjectResult(result.Data) { StatusCode = result.StatusCode };

        return new ObjectResult(new { error = result.Error }) { StatusCode = result.StatusCode };
    }

    
    ///<summary>Errorhandler wrapper</summary>
    ///<returns>HTTP Response</returns>
    public static IActionResult SafeExecute<T>(Func<ServiceResult<T>> action){
        try{
            return action().ToResponse();
        }
        catch (ArgumentException ex){
            Console.WriteLine($"[Error] {ex.Message}");
            return ServiceResult<T>.BadRequest(ex.Message).ToResponse();
        }
        catch (KeyNotFoundException ex){
            Console.WriteLine($"[Error] {ex.Message}");
            return ServiceResult<T>.NotFound(ex.Message).ToResponse();
        }
        catch(HttpRequestException ex){
            Console.WriteLine($"[Error] {ex.Message}");
            return ServiceResult<T>.Upstream(ex.Message, (int)(ex.StatusCode ?? System.Net.HttpStatusCode.BadRequest)).ToResponse();
        }
        catch (Exception ex){
            Console.WriteLine($"[Error] {ex.Message}");
            return ServiceResult<T>.InternalError(ex.Message).ToResponse();
        }
    }
}
