namespace BookStore.Ordering.Application.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(
        T message,
        string queueName)
        where T : class;
}