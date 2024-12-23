using lockbox_user_service.Models;

namespace lockbox_user_service.Services;

public class MessageService : IMessageService
{
    public Message GetAdminMessage()
    {
        return new Message { text = "This is an admin message." };
    }

    public Message GetProtectedMessage()
    {
        return new Message { text = "This is a protected message." };
    }

    public Message GetPublicMessage()
    {
        return new Message { text = "This is a public message." };
    }
}
