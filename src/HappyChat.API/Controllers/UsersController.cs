using HappyChat.Application.Users.DTOs;
using HappyChat.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyChat.API.Controllers;

/// <summary>Cadastro, verificação de e-mail e consulta de usuários.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Inicia o cadastro: envia um código de 6 dígitos por e-mail.</summary>
    /// <remarks>
    /// Nenhuma conta é criada nesta etapa: os dados ficam pendentes até a confirmação
    /// do código em <c>POST /api/users/verify</c>. O código vale por tempo limitado.
    /// Chamar este endpoint novamente para o mesmo e-mail gera um novo código.
    /// </remarks>
    /// <param name="request">Dados do novo usuário.</param>
    /// <response code="202">Código de verificação enviado para o e-mail informado.</response>
    /// <response code="400">Dados inválidos (campo obrigatório ausente, formato ou tamanho incorreto).</response>
    /// <response code="409">E-mail ou perfil já cadastrado.</response>
    /// <response code="503">Falha ao enviar o e-mail de verificação.</response>
    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        await _userService.StartRegistrationAsync(request);
        return Accepted(new MessageResponse("Enviamos um código de verificação para o seu e-mail."));
    }

    /// <summary>Confirma o código de verificação e cria a conta.</summary>
    /// <remarks>
    /// Requer um cadastro pendente iniciado em <c>POST /api/users</c>.
    /// O código tem 6 dígitos e permite um número limitado de tentativas incorretas.
    /// Em caso de sucesso, a conta é criada e um e-mail de boas-vindas é enviado.
    /// </remarks>
    /// <param name="request">E-mail do cadastro e código de 6 dígitos recebido.</param>
    /// <response code="201">Código válido: conta criada. O header <c>Location</c> aponta para o usuário.</response>
    /// <response code="400">Formato inválido, código incorreto, ou código expirado / cadastro não encontrado.</response>
    /// <response code="409">E-mail ou perfil foi registrado por outra conta durante a verificação.</response>
    /// <response code="429">Limite de tentativas incorretas atingido; é preciso refazer o cadastro.</response>
    [HttpPost("verify")]
    [AllowAnonymous]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserResponse>> Verify(VerifyEmailRequest request)
    {
        var user = await _userService.ConfirmRegistrationAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Busca um usuário pelo id.</summary>
    /// <remarks>Requer token JWT válido.</remarks>
    /// <param name="id">Identificador (GUID) do usuário.</param>
    /// <response code="200">Usuário encontrado.</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    /// <response code="404">Nenhum usuário com este id.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Lista todos os usuários.</summary>
    /// <remarks>Requer token JWT válido. A lista não é paginada.</remarks>
    /// <response code="200">Lista de usuários (vazia se não houver nenhum).</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
}