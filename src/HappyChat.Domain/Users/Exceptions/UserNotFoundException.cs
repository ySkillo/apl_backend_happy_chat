using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException() : base("Usuário não encontrado.") {}
}