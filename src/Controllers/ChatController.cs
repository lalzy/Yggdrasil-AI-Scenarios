// ChatController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Util;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _service;

    public ChatController(ChatService service)
    {
        _service = service;
    }
    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] SendRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Send(request.Connection, request.Payload).GetAwaiter().GetResult());
}
