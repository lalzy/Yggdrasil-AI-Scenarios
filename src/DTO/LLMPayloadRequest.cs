// LLMPayloadRequest.cs

using Yggdrasil.Models;

namespace Yggdrasil.DTO;

public class LLMPayloadRequest{
    public World World { get; set; }
    public Persona Persona { get; set; }
    public List<Message>? Messages { get; set; }
}
