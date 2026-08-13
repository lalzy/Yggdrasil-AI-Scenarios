// ConversationController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/{controller}")]
public class ConversationController : ControllerBase{
    private readonly ConversationService _service;

    public ConversationController(ConversationService service){
        _service = service;
    }

    [HttpGet("all/{world_ID}")]
    public IActionResult GetAllFromWorld(Guid world_ID, [FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAllFromWorld(world_ID, count));
}
