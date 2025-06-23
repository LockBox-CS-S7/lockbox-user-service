namespace lockbox_user_service.Messaging;

public class AccountMessage
{
    public string EventType { get; set; }
    public string Timestamp { get; set; }
    public string Source { get; } = "user-service";
    public string UserId { get; set; }
    public string Body { get; set; }

    public AccountMessage(string eventType, string timestamp, string userId, string body)
    {
        EventType = eventType;
        Timestamp = timestamp;
        UserId = userId;
        Body = body;
    }
}