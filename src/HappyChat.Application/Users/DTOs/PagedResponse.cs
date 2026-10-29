namespace HappyChat.Application.Users.DTOs;

/// <summary>Página de resultados.</summary>
public class PagedResponse<T>
{
    /// <summary>Itens da página atual.</summary>
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <summary>Número da página (começa em 1).</summary>
    public int Page { get; set; }

    /// <summary>Quantidade máxima de itens por página.</summary>
    public int PageSize { get; set; }

    /// <summary>Total de itens em todas as páginas.</summary>
    public int TotalCount { get; set; }

    /// <summary>Total de páginas.</summary>
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}