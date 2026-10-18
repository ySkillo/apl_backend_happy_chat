namespace HappyChat.Application.Users.Interfaces;

public interface IEmailNotifier
{
    Task SendAccountOpeningAsync(string to, string name, CancellationToken ct = default);
}