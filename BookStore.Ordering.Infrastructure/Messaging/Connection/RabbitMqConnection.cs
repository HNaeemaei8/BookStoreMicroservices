using BookStore.Ordering.Infrastructure.Messaging.Connection;
using RabbitMQ.Client;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(IConnectionFactory factory)
    {
        _factory = factory;
    }

    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = _factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
        }

        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}