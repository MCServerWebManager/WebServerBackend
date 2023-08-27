using MCServerWebManagerBackend.Data;
using MCServerWebManagerBackend.Data.Models;
using MCServerWebManagerBackend.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCServerWebManagerBackend.Controllers;

[ApiController]
public class AuthController
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _repo;
    
    public AuthController(ILogger<AuthController> logger, IUserRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    /// <summary>
    /// 用户登录API, 假如登录成功则返回 登录token
    /// </summary>
    [HttpPost]
    [Route("/Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageContainer<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(MessageContainer<string>))]
    public async Task<IResult> Login([FromBody] LoginInputModel input)
    {
        var msg = await _repo.Login(input.UserName, input.Password);
        
        return msg.Success ? Results.Ok(msg) :  Results.BadRequest(msg);
    }


    /// <summary>
    /// 用于测试是否登录的API, 假如 Authorization 是正确的则返回 http码 200
    /// </summary>
    [HttpGet]
    [Route("/LoggedIn")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IResult> LoggedIn()
    {
        return Task.FromResult(Results.Ok());
    }
}