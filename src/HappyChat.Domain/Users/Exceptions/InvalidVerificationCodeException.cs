using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class InvalidVerificationCodeException : DomainException
{
    public InvalidVerificationCodeException() : base("Código de verificação inválido.") { }
}