using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace lockbox_user_service.Messaging;

public class RabbitMqClient : IDisposable
{
    private string _queueName;
    private IConnection? _connection;
    private IChannel? _channel;

    private RabbitMqClient(string queueName, IConnection? connection, IChannel? channel)
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
    public async Task<RabbitMqClient> CreateAsync(string queueName)
    {
        _queueName = queueName;
        string rabbitUri = Environment.GetEnvironmentVariable("RABBITMQ_CONN_STRING") ??
                           throw new Exception("Could not get the RabbitMQ broker uri from environment.");

        var factory = new ConnectionFactory { Uri = new Uri(rabbitUri) };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.QueueDeclareAsync(_queueName, true, false, false, null);

        var instance = new RabbitMqClient(_queueName, _connection, _channel);
        return instance;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }
}