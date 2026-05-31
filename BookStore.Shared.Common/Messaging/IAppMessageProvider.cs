namespace BookStore.Shared.Common.Messaging;

public interface IAppMessageProvider
{
    string GetMessage(string code, string? fallbackMessage = null);
}
