// LLMConnectionDTO.cs

using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Models.DTO;

public class LLMConnectionDTO{
    public record Request(
        [Required] string Name,
        [Required] string RequestFormat,
        [Required] string ResponseFormat,
        string? Headers,
        [Required] string URL,
        int? Port,
        string? Model,
        string? APIKey
        
    );
    public record Update(
        string? Name=null,
        string? RequestFormat=null,
        string? ResponseFormat=null,
        string? Headers=null,
        string? URL=null,
        int? Port=null,
        string? Model=null,
        string? APIKey=null
    );
}
