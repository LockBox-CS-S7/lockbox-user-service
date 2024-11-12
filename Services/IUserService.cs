using lockbox_user_service.Models;

namespace lockbox_user_service.Services;

public interface IUserService
{
    Task<string?> GetUserById(string id);
    Task<bool> DeleteUserById(string id);
}