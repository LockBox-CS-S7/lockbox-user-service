using System.Net.Mime;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace lockbox_user_service.Messaging;

public class RabbitMqClient : IDisposable
{
    private string _queueName;
    private IConnection _connection;
    private IChannel _channel;
    
    private RabbitMqClient(string queueName, IConnection connection, IChannel channel)
    {
        _queueName = queueName;
        _connection = connection;
        _channel = channel;
    }
    
    /// <summary>
    /// Functions as an asynchronous constructor for the RabbitMqClient.
    /// </summary>
    /// <param name="queueName">The name of the queue the messages should be sent to.</param>
    /// <returns>A new instance of the RabbitMqClient class.</returns>
    public static async Task<RabbitMqClient> CreateAsync(string queueName)
    {
        string rabbitUri = Environment.GetEnvironmentVariable("RABBITMQ_URI") ??
                           throw new Exception("Could not get the RabbitMQ broker uri from environment.");

        var factory = new ConnectionFactory { Uri = new Uri(rabbitUri) };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queueName, true, false, false, null);

        var instance = new RabbitMqClient(queueName, connection, channel);
        return instance;
    }
    
    public async Task SendMessageAsync(AccountMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        await _channel.BasicPublishAsync(string.Empty, _queueName, body);
        Console.WriteLine($" [x] Send message: {message}");
    }
    
    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}