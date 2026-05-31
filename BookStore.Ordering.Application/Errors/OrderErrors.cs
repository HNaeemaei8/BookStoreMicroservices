using BookStore.Shared.Common.Results;

namespace BookStore.Ordering.Application.Errors;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) =>
        new("Ordering.Order.NotFound", $"Order with id '{orderId}' was not found.", ErrorType.NotFound);

    public static Error InvalidQuantity(int quantity) =>
        new("Ordering.Order.InvalidQuantity", $"Quantity '{quantity}' is invalid.", ErrorType.Validation);
}
