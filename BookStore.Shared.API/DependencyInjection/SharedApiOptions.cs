namespace BookStore.Shared.API.DependencyInjection;

public sealed class SharedApiOptions
{
    public IDictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();
}
