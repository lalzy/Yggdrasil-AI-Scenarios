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

    [HttpGet("{connection_ID}")]
    public IActionResult GetOne(Guid connection_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetOne(connection_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] LLMConnectionRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Create(request));

    [HttpDelete("{connection_ID}")]
    public IActionResult Delete(Guid connection_ID) => ServiceResultExtensions.SafeExecute(() => _service.Delete(connection_ID));
}
