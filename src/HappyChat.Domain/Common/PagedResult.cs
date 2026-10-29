namespace HappyChat.Domain.Common;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);