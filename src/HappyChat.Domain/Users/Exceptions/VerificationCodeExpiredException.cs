using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class VerificationCodeExpiredException : DomainException
{
    public VerificationCodeExpiredException()
        : base("Código expirado ou cadastro não encontrado. Faça o cadastro novamente.") { }
}