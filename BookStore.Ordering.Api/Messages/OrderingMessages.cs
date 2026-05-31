namespace BookStore.Modules.Ordering.API;

public static class OrderingMessages
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Ordering.Order.NotFound"] = "سفارش موردنظر پیدا نشد.",
            ["Ordering.OrderItem.NotFound"] = "آیتم سفارش پیدا نشد."
        };
}
