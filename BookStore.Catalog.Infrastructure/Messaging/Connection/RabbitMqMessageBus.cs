using BookStore.Catalog.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BookStore.Catalog.Infrastructure.Messaging.Connection;

public class RabbitMqMessageBus
    : IMessageBus
{
    private readonly IConnection _connection;

    public RabbitMqMessageBus(
        IRabbitMqConnection rabbitMqConnection)
    {
        _connection =
            rabbitMqConnection.GetConnection();
    }

    public async Task PublishAsync<T>(
        T message,
        string queueName)
        where T : class
    {
        await using var channel =
            await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json =
            JsonSerializer.Serialize(message);

        var body =
            Encoding.UTF8.GetBytes(json);

        var properties =
            new BasicProperties
            {
                Persistent = true
            };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}