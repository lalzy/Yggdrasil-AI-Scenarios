// LLMConnectionRequest

using System.ComponentModel.DataAnnotations;
using Yggdrasil.Models;

namespace Yggdrasil.DTO;

public class LLMConnectionRequest
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public required APIType APIType  { get; set; }
    [Required]
    public required string URL  { get; set; }
    [Required]
    public required SupportedProviders Provider {get; set;}
    public string? APIKey  { get; set; }
    public string? Model  { get; set; }
    public bool Reasoning { get; set; }
}
