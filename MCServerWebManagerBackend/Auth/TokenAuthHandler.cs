using System.Security.Claims;
using System.Text.Encodings.Web;
using MCServerWebManagerBackend.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace MCServerWebManagerBackend.Auth;
public class TokenAuthHandler : AuthenticationHandler<TokenAuthOptions>
{
    private readonly IUserRepository _repo;
    private readonly AppSettings _settings;
    
    public TokenAuthHandler(AppSettings settings, IUserRepository repo, IOptionsMonitor<TokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _repo = repo;
        _settings = settings;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //检测是否有token
        if (!Request.Headers.ContainsKey("X-User-Token"))
            return AuthenticateResult.Fail("Header not found");

        var token = Request.Headers["X-User-Token"].ToString();
        
        
        //检测token是否合法
        var message = await _repo.GetUserFromToken(token, _settings.Login.TokenExpireTime);

        if (!message.Success)
            return AuthenticateResult.Fail(message.Message ?? "");
        
        
        //生成 authentication token 并返回
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, message.Data!.UserName),
            new Claim(ClaimTypes.NameIdentifier, message.Data!.Id.ToString())
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Token"));
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}