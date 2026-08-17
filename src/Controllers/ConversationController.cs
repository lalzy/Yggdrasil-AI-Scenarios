// ConversationController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase{
    private readonly ConversationService _service;

    public ConversationController(ConversationService service){
        _service = service;
    }

    [HttpGet("all/{world_ID}")]
    public IActionResult GetAllFromWorld(Guid world_ID, [FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAllFromWorld(world_ID, count));

    [HttpGet("{conversation_ID}")]
    public IActionResult GetOne(Guid conversation_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetOne(conversation_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] ConversationRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Create(request));

    [HttpDelete("{conversation_ID}")]
    public IActionResult Delete(Guid conversation_ID) => ServiceResultExtensions.SafeExecute(() => _service.Delete(conversation_ID));
}
