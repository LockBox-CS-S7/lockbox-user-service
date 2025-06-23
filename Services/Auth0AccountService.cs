using lockbox_user_service.Messaging;

namespace lockbox_user_service.Services;

/// <summary>
/// This is a mock service for demonstration purposes at the moment. It should become functional at some point.
/// </summary>
public class Auth0AccountService : IAccountService
{
    private const string AccountQueue = "account-queue";
    
    public async Task DeleteUserAccount(string id)
    {
        Console.WriteLine($"Deleted user account with id: {id}");
        using RabbitMqClient client = await RabbitMqClient.CreateAsync(AccountQueue);

        var now = DateTime.UtcNow;
        var message = new AccountMessage(
            "ACCOUNT_DELETION_REQUESTED", 
            $"{now.ToLongTimeString()} - {now.ToLongDateString()}", 
            id, 
            "The user has requested their account to be deleted.");
        await client.SendMessageAsync(message);
    }
}
