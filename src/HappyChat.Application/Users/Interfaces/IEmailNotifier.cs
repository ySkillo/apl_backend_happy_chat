namespace HappyChat.Application.Users.Interfaces;

public interface IEmailNotifier
{
    Task SendAccountOpeningAsync(string to, string name, CancellationToken ct = default);
    Task SendVerificationCodeAsync(string to, string name, string code, CancellationToken ct = default);
}