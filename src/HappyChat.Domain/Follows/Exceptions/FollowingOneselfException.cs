using HappyChat.Domain.Common;

namespace HappyChat.Domain.Follows.Exceptions;

public class FollowingOneselfException : DomainException
{
    public FollowingOneselfException() : base("Não é possivel seguir você mesmo!") {}
}