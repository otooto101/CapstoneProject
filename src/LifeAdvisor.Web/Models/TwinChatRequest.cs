namespace LifeAdvisor.Web.Models;

// JSON body posted by the dashboard chat widget. History is the running conversation so the
// chat can stay stateless on the server.
public class TwinChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<TwinChatTurn> History { get; set; } = new();
}

public class TwinChatTurn
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
}
