using HappyChat.Domain.Common;

namespace HappyChat.Domain.Follows.Exceptions;

public class NotFollowingException : DomainException
{
    public NotFollowingException() : base("Você não segue este usuário.") {}
}