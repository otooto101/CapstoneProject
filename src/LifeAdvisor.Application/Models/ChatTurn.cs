namespace LifeAdvisor.Application.Models;

// One message in an interactive twin conversation. Role is "user" or "assistant".
public record ChatTurn(string Role, string Content);
