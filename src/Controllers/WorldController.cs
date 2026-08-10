// WorldController.cs

using Yggdrasil.DTO;
using Yggdrasil.Services;
using Yggdrasil.Extensions;

namespace Yggdrasil.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorldController : ControllerBase{
    private readonly WorldService _service;

    public WorldController(WorldService service){
        _service = service;
    }

    [HttpGet("all")]
    public IActionResult GetAll([FromQuery] int? count) => ServiceResultExtensions.SafeExecute(() => _service.GetAll(count));

    [HttpGet("{world_ID}")]
    public IActionResult GetOne(Guid world_ID) => ServiceResultExtensions.SafeExecute(() => _service.GetOne(world_ID));

    [HttpPost("create")]
    public IActionResult Create([FromBody] WorldRequest request) => ServiceResultExtensions.SafeExecute(() => _service.Create(request));


    [HttpDelete("{world_ID}")]
    public async Task<IActionResult> Delete(Guid world_ID) => ServiceResultExtensions.SafeExecute(() => _service.Delete(world_ID));

    [HttpPost("{world_ID}/characters/{character_ID}")]
    public IActionResult AddCharacter(Guid world_ID, Guid character_ID) => ServiceResultExtensions.SafeExecute(() => _service.AddCharacter(world_ID, character_ID));

    [HttpDelete("{world_ID}/characters/{character_ID}")]
    public IActionResult RemoveCharacter(Guid world_ID, Guid character_ID) => ServiceResultExtensions.SafeExecute(() => _service.RemoveCharacter(world_ID, character_ID));
}
