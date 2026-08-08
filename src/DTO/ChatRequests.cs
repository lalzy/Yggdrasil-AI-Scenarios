// ChatRequests.cs

using Yggdrasil.Models;

namespace Yggdrasil.DTO;

public class SendRequest{
    public LLMConnectionRequest Connection { get; set; }
    public LLMPayload Payload { get; set; }
}
