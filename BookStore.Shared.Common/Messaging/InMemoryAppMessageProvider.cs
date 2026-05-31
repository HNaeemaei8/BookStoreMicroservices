using BookStore.Shared.Common.Messaging;

namespace BookStore.Shared.API.Messaging;

public sealed class InMemoryAppMessageProvider : IAppMessageProvider
{
    private readonly Dictionary<string, string> _messages;

    public InMemoryAppMessageProvider(IDictionary<string, string> messages)
    {
        _messages = new Dictionary<string, string>(messages);
    }

    public string GetMessage(string code, string? fallbackMessage = null)
    {
        if (!string.IsNullOrEmpty(code) && _messages.TryGetValue(code, out var message))
        {
            return message;
        }

        return fallbackMessage ?? "An unexpected error occurred.";
    }
}
