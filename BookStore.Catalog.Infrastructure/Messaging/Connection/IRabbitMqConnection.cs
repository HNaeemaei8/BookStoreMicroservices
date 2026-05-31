using RabbitMQ.Client;

namespace BookStore.Catalog.Infrastructure.Messaging.Connection;

public interface IRabbitMqConnection
{
    IConnection GetConnection();
}