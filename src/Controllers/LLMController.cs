// LLMController.cs

using Yggdrasil.Models;
using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LLMController : ControllerBase{
    private readonly LLMService _service;

    public LLMController(LLMService service){
        _service = service;
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] LLMPayloadRequest request) => ServiceResultExtensions.SafeExecute(() => _service.CreateLLMPayload(request.World, request.Persona, request.Messages));
}
