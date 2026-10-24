using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HappyChat.Application.Auth.DTOs;
using HappyChat.Application.Auth.Interfaces;
using HappyChat.Application.Users.DTOs;
using HappyChat.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyChat.API.Controllers;

/// <summary>Autenticação e identificação do usuário logado.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>Autentica o usuário e devolve um token JWT.</summary>
    /// <remarks>
    /// Use o valor de <c>accessToken</c> no header <c>Authorization: Bearer {token}</c>
    /// para acessar os endpoints protegidos. No Swagger, cole o token no botão
    /// <b>Authorize</b> (sem o prefixo "Bearer ").
    /// </remarks>
    /// <param name="request">E-mail e senha do usuário.</param>
    /// <response code="200">Login realizado: token JWT, data de expiração e dados do usuário.</response>
    /// <response code="400">Dados inválidos (campo obrigatório ausente ou formato incorreto).</response>
    /// <response code="401">E-mail ou senha inválidos, ou conta inativa.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>Devolve o usuário dono do token enviado.</summary>
    /// <remarks>
    /// Útil para o front recuperar os dados do usuário logado a partir do token,
    /// por exemplo ao reabrir o app. Requer token JWT válido.
    /// </remarks>
    /// <response code="200">Dados do usuário autenticado.</response>
    /// <response code="401">Token ausente, inválido ou expirado, ou usuário do token não existe mais.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Me()
    {
        if (!TryGetUserId(out var id))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(id);
        return user is null ? Unauthorized() : Ok(user);
    }

    /// <summary>Encerra a sessão do usuário logado.</summary>
    /// <remarks>
    /// Marca o usuário como offline (<c>isOnline = false</c>). O front deve descartar o token;
    /// ele continua tecnicamente válido até expirar. Requer token JWT válido.
    /// </remarks>
    /// <response code="204">Sessão encerrada.</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        if (!TryGetUserId(out var id))
            return Unauthorized();

        await _authService.LogoutAsync(id);
        return NoContent();
    }

    private bool TryGetUserId(out long id)
    {
        // Dependendo do mapeamento de claims, o "sub" chega com um dos dois nomes.
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return long.TryParse(sub, out id);
    }
}