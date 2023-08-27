using MCServerWebManagerBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace MCServerWebManagerBackend.Data;

public interface IUserRepository
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>如成功则返回用户token</returns>
    public Task<MessageContainer<string>> Login(string username, string password);

    /// <summary>
    /// 通过用户Token来获取用户信息
    /// </summary>
    /// <param name="token">用户token</param>
    /// <param name="expireTime">token有效时长，单位为秒。默认为-1(无限制)，</param>
    /// <returns>如成功则返回用户信息</returns>
    public Task<MessageContainer<User>> GetUserFromToken(string token, long expireTime = -1);
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

        //写入sqlite 并返回token
        await _context.Tokens.AddAsync(token);
        await _context.Ctx.SaveChangesAsync();
        
        ret.Data = token.TokenStr;
        ret.Success = true;
        _logger.LogDebug($"用户: {user.UserName} 登录成功");
        
        return ret;
    }

    public async Task<MessageContainer<User>> GetUserFromToken(string token, long expireTime = -1)
    {
        var userInfo = await (
            from tokenRec in _context.Tokens
            join userRec in _context.Users
            on tokenRec.User equals userRec.Id
            where tokenRec.TokenStr == token
            select new
            {
                CreatedAt = tokenRec.CreatedAt,
                UserName = userRec.UserName,
                Password = userRec.Password,
                UserId = userRec.Id
            }
        ).FirstOrDefaultAsync();

        
        //没找到对应的User
        if (userInfo is null)
        {
            return new MessageContainer<User>()
            {
                Message = "Token不正确",
                Data = null,
                Success = false
            };
        }

        
        var user = new User() { Id = userInfo.UserId, UserName = userInfo.UserName, Password = userInfo.Password };
        
        //Token过期
        if ((expireTime != -1) && userInfo.CreatedAt.AddSeconds(expireTime) < DateTime.Now)
        {
            return new MessageContainer<User>()
            {
                Message = "Token已过期",
                Data = null,
                Success = false
            };
        }

        
        return new MessageContainer<User>()
        {
            Success = true,
            Data = user
        };
    }
}