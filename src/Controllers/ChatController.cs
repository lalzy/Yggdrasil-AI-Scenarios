// ChatMessageController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase{
    private readonly ChatService _service;

    public ChatController(ChatService service){
        _service = service;
    }

    [HttpGet("all/{conversation_ID}")]
    public IActionResult GetAll(Guid conversation_ID, [FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(conversation_ID, count));

    [HttpGet("{chatMessage_ID}")]
    public IActionResult GetOne(Guid chatMessage_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(chatMessage_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] ChatMessageRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Create(request));

    [HttpPatch("{chatMessage_ID}")]
    public IActionResult Update(Guid chatMessage_ID ,[FromBody] ChatMessageUpdateRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Update(chatMessage_ID, request));

    [HttpDelete("{chatMessage_ID}")]
    public IActionResult Delete(Guid chatMessage_ID) => ServiceResultExtensions.SafeExecute(() => _service.Delete(chatMessage_ID));
}
