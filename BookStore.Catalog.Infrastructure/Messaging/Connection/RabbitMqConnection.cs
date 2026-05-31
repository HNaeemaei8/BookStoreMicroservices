using RabbitMQ.Client;

namespace BookStore.Catalog.Infrastructure.Messaging.Connection;

public class RabbitMqConnection
    : IRabbitMqConnection, IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqConnection(
        string host,
        int port,
        string username,
        string password)
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
                   .GetResult();
    }

    public IConnection GetConnection()
    {
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}