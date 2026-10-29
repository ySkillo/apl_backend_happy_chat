using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class NotFollowingException : DomainException
{
    public NotFollowingException() : base("Você não segue este usuário.") {}
}