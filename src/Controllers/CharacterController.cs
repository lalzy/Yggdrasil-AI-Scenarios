// CharacterController.cs

using Yggdrasil.Services;
using Yggdrasil.Extensions;
using Yggdrasil.DTO;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CharacterController : ControllerBase{
    private readonly CharacterService _service;
    public CharacterController(CharacterService service){
        _service = service;
    }

    [HttpGet("all")]
    public IActionResult GetAll([FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(count));
    
    [HttpGet("{character_ID}")]
    public IActionResult GetOne(Guid character_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetOne(character_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] CharacterRequest request) => ServiceResultExtensions.SafeExecute(()=> _service.Create(request));

    [HttpDelete("{character_ID}")]
    public IActionResult Delete(Guid character_ID) => ServiceResultExtensions.SafeExecute(()=> _service.Delete(character_ID));
}
