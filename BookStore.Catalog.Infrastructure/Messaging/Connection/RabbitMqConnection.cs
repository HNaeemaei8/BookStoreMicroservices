using BookStore.Catalog.Infrastructure.Messaging.Connection;
using RabbitMQ.Client;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;
    private readonly object _lock = new();

    public RabbitMqConnection(IConnectionFactory factory)
    {
        _factory = factory;
    }

    public IConnection GetConnection()
    {
        if (_connection != null && _connection.IsOpen)
            return _connection;

        lock (_lock)
        {
            if (_connection != null && _connection.IsOpen)
                return _connection;

            var retryCount = 0;

            while (true)
            {
                try
                {
                    _connection = _factory.CreateConnectionAsync()
                        .GetAwaiter()
                        .GetResult();

                    return _connection;
                }
                catch
                {
                    retryCount++;

                    if (retryCount > 10)
                        throw;

                    Thread.Sleep(3000); 
                }
            }
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}