// LLMSendController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/chat")] // Want code separation, but the route to point to chat.
public class LLMSendController : ControllerBase
{
    private readonly LLMSendService _service;

    public LLMSendController(LLMSendService service)
    {
        _service = service;
    }
    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] SendRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Send(request.Connection, request.Payload).Result);
}
