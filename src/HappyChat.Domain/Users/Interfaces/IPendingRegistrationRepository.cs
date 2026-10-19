using HappyChat.Domain.Users.Entities;

namespace HappyChat.Domain.Users.Interfaces;

public interface IPendingRegistrationRepository
{
    /// <summary>Salva o pendente; se já existir um para o e-mail, sobrescreve.</summary>
    Task SaveAsync(PendingRegistration pending);
    Task<PendingRegistration?> GetByEmailAsync(string email);
    /// <summary>Soma 1 nas tentativas e devolve o total.</summary>
    Task<int> RegisterFailedAttemptAsync(string email);
    Task RemoveAsync(string email);
}