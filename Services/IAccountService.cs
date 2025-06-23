namespace lockbox_user_service.Services;

public interface IAccountService
{
    public Task DeleteUserAccount(string id);
}