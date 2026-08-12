// ChatMessageController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatMessageController : ControllerBase{
    private readonly ChatService _service;

    public ChatMessageController(ChatService service){
        _service = service;
    }

    [HttpGet("all/{conversation_ID}")]
    public IActionResult GetAll(Guid conversation_ID, [FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(conversation_ID, count));
}
