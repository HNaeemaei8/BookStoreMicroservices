using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderService.Application.Interfaces;

namespace BookStore.Ordering.Infrastructure.Messaging.Connection;

public class RabbitMqMessageBus : IMessageBus
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
        string queueName,
        string exchangeName = "")
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
            exchange: exchangeName,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}