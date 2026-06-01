using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Infrastructure.Messaging.Connection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public RabbitMqMessageBus(IRabbitMqConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async Task PublishAsync<T>(T message, string queueName)
        where T : class
    {
        var connection = _rabbitMqConnection.GetConnection();

        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
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