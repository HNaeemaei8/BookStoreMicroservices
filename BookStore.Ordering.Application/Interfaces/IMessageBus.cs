namespace OrderService.Application.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string queueName, string exchangeName = "") where T : class;
}
