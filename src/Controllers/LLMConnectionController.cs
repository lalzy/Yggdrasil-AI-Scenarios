// LLMConnectioncontroller.cs

using Yggdrasil.Constants;
using Yggdrasil.Models;
using Yggdrasil.Util;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Extensions;

namespace Yggdrasil.Services;

[ApiController]
[Route("api/connection")]
public class LLMConnectionController : ControllerBase{
    private readonly LLMConnectionService _service;

    public LLMConnectionController(LLMConnectionService service){
        _service = service;
    }

    [HttpGet("all")]
    public IActionResult GetAll([FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(count));
}
