using MCServerWebManagerBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace MCServerWebManagerBackend.Data;

public interface IUserRepository
{
    public Task<MessageContainer<string>> Login(string username, string password);
}

public class UserRepository : IUserRepository
{
    private readonly IContext _context;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(IContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MessageContainer<string>> Login(string username, string password)
    {
        var ret = new MessageContainer<string>();
        var user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

        //判断用户是否存在
        if (user is null)
        {
            ret.Message = "账号或密码不正确";
            ret.Success = false;
            return ret;
        }

        //判断密码是否正确
        if (user.Password != password)
        {
            ret.Message = "账号或密码不正确";
            ret.Success = false;
            return ret;
        }
        
        //认证成功，创建一个新的token并返回
        var token = new Token()
        {
            User = user.Id,
            TokenStr = Extensions.GenerateToken()
        };
        ret.ReturnData = token.TokenStr;

        //写入sqlite
        await _context.Tokens.AddAsync(token);
        await _context.Ctx.SaveChangesAsync();
        
        _logger.LogInformation($"用户: {user.UserName} 登录成功, 登录Token: {ret.ReturnData}");
        
        return ret;
    }
}