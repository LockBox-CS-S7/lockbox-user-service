using lockbox_file_service.Models;

namespace lockbox_file_service.Services;

public interface IMessageService
{
    Message GetPublicMessage();
    Message GetProtectedMessage();
    Message GetAdminMessage();
}