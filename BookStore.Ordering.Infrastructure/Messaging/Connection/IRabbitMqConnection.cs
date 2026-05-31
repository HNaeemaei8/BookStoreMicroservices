using RabbitMQ.Client;

namespace BookStore.Ordering.Infrastructure.Messaging.Connection;

public interface IRabbitMqConnection
{
    IConnection GetConnection();
}

