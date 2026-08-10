// PersonaController.cs

using Yggdrasil.Extensions;
using Yggdrasil.Services;
using Yggdrasil.DTO;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PersonaController : ControllerBase{
    private readonly PersonaService _service;
    public PersonaController(PersonaService service){
        _service = service;
    }

    [HttpGet("all")]
    public IActionResult GetAll([FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(count));

    [HttpGet("{persona_ID}")]
    public IActionResult GetOne(Guid persona_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetOne(persona_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] PersonaRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Create(request));

    [HttpDelete("{persona_ID}")]
    public IActionResult Delete(Guid persona_ID) => ServiceResultExtensions.SafeExecute(() => _service.Delete(persona_ID));
}
