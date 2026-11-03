using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HappyChat.Application.Common.DTOs;
using HappyChat.Application.Follows.DTOs;
using HappyChat.Application.Follows.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyChat.API.Controllers;

/// <summary>Seguir, deixar de seguir e listar seguidores/seguidos.</summary>
/// <remarks>
/// As rotas continuam sob <c>api/users/{id}/...</c>: o follow é uma ação sobre um usuário,
/// mas vive em um controller próprio.
/// </remarks>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize]
public class FollowsController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowsController(IFollowService followService)
    {
        _followService = followService;
    }

    /// <summary>Passa a seguir um usuário.</summary>
    /// <remarks>Requer token JWT válido. Quem segue é o usuário do token.</remarks>
    /// <param name="id">Identificador do usuário a ser seguido.</param>
    /// <response code="204">Agora você segue este usuário.</response>
    /// <response code="400">Tentativa de seguir a si mesmo.</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    /// <response code="404">Usuário a ser seguido não existe.</response>
    /// <response code="409">Você já segue este usuário.</response>
    [HttpPost("{id:long}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(long id)
    {
        if (!TryGetUserId(out var followerId))
            return Unauthorized();

        await _followService.FollowAsync(followerId, id);
        return NoContent();
    }

    /// <summary>Deixa de seguir um usuário.</summary>
    /// <remarks>Requer token JWT válido.</remarks>
    /// <param name="id">Identificador do usuário a deixar de seguir.</param>
    /// <response code="204">Você deixou de seguir este usuário.</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    /// <response code="404">Você não segue este usuário.</response>
    [HttpDelete("{id:long}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unfollow(long id)
    {
        if (!TryGetUserId(out var followerId))
            return Unauthorized();

        await _followService.UnfollowAsync(followerId, id);
        return NoContent();
    }

    /// <summary>Lista quem segue o usuário (seguidores).</summary>
    /// <remarks>Requer token JWT válido. Paginado, do follow mais recente para o mais antigo.</remarks>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="page">Número da página, começando em 1.</param>
    /// <param name="pageSize">Itens por página (padrão 20, máximo 50).</param>
    /// <response code="200">Página de seguidores (vazia se não houver nenhum).</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    /// <response code="404">Usuário não existe.</response>
    [HttpGet("{id:long}/followers")]
    [ProducesResponseType(typeof(PagedResponse<FollowUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<FollowUserResponse>>> GetFollowers(
        long id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _followService.GetFollowersAsync(id, page, pageSize);
        return Ok(result);
    }

    /// <summary>Lista quem o usuário segue.</summary>
    /// <remarks>Requer token JWT válido. Paginado, do follow mais recente para o mais antigo.</remarks>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="page">Número da página, começando em 1.</param>
    /// <param name="pageSize">Itens por página (padrão 20, máximo 50).</param>
    /// <response code="200">Página de usuários seguidos (vazia se não seguir ninguém).</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    /// <response code="404">Usuário não existe.</response>
    [HttpGet("{id:long}/following")]
    [ProducesResponseType(typeof(PagedResponse<FollowUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<FollowUserResponse>>> GetFollowing(
        long id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _followService.GetFollowingAsync(id, page, pageSize);
        return Ok(result);
    }

    private bool TryGetUserId(out long id)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return long.TryParse(sub, out id);
    }
}
