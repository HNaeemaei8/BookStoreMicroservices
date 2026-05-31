using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BookStore.Ordering.Infrastructure.Messaging.Connection;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqConnection(
        string host,
        int port,
        string username,
        string password,
        ILogger<RabbitMqConnection> logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password
        };

        _connection =
            factory.CreateConnectionAsync()
                   .GetAwaiter()
                   .GetResult();    }


    public IConnection GetConnection()
    {
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}