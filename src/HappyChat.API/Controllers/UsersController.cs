using HappyChat.Application.Users.DTOs;
using HappyChat.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyChat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Inicia o cadastro: envia um código de 6 dígitos por e-mail.</summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        await _userService.StartRegistrationAsync(request);
        return Accepted(new { message = "Enviamos um código de verificação para o seu e-mail." });
    }

    /// <summary>Confirma o código e cria a conta.</summary>
    [HttpPost("verify")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserResponse>> Verify(VerifyEmailRequest request)
    {
        var user = await _userService.ConfirmRegistrationAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Busca um usuário pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Lista todos os usuários.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
}
