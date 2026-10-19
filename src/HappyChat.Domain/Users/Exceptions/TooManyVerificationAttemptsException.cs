using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class TooManyVerificationAttemptsException : DomainException
{
    public TooManyVerificationAttemptsException()
        : base("Muitas tentativas incorretas. Faça o cadastro novamente para receber um novo código.") { }
}