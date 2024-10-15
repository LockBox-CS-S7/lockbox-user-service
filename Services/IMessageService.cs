using lockbox_user_service.Models;

namespace lockbox_user_service.Services;

public interface IMessageService
{
    Message GetPublicMessage();
    Message GetProtectedMessage();
    Message GetAdminMessage();
}