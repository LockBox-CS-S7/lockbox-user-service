namespace lockbox_user_service.Services;

/// <summary>
/// This is a mock service for demonstration purposes at the moment. It should become functional at some point.
/// </summary>
public class Auth0AccountService : IAccountService
{
    public void DeleteUserAccount(string id)
    {
        Console.WriteLine($"Deleted user account with id: {id}");
        
        // TODO: Send message to RabbitMQ broker (CloudAMQP)
    }
}
