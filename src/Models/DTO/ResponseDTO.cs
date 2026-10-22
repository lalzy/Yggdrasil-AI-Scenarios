// ResponseDTO.cs

namespace Yggdrasil.Models.DTO;

public class ResponseDTO{
    public DateTime? Created;
    public required string Model;
    public required MessageDTO Message;
    public required string FinishReason;
    public UsageDTO? Usage;
    public TimingsDTO? Timings;
}
