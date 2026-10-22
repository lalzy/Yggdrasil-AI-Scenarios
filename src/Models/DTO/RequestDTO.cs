// RequestDTO.cs

namespace Yggdrasil.Models.DTO;

public class RequestDTO{
    public string? model;
    
    public List<MessageDTO> Messages = [];

    // Output
    public int MaxContext;
    public int MaxTokens;
    public bool Streaming; // response bit by bit
    public bool Reasoning;
    public string? ReasoningEffort;

    // Sampling
    public double Temperature;
    public double TopP;
    public int Seed;
    public double FrequencyPenalty;
    public double PresencePenalty;
}
