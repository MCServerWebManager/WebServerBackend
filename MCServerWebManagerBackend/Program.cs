using MCServerWebManagerBackend.Data;
using MCServerWebManagerBackend.Data.Models;
using MCServerWebManagerBackend.Socket;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
logger.Info("Starting...");


//自定义Services


//Nlog Services
builder.Logging.ClearProviders();
builder.Host.UseNLog();

//网页后端Services
builder.Services.AddHostedService<SocketFactory>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//配置sqlite
builder.Services.AddDbContext<IContext, Context>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("database")));



try
{
    var app = builder.Build();

    //假如数据库不存在就创建一个新的
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<IContext>();
        context.Ctx.Database.EnsureCreated();
        
        
        //假设users 表是空的 就默认创建一个 账号为root 密码为password的账号
        if ((await context.Users.CountAsync()) < 1)
        {
            var rootUser = new User()
            {
                UserName = "root",
                Password = ""
            };

            await context.Users.AddAsync(rootUser);
            await context.Ctx.SaveChangesAsync();
            
            logger.Info($"默认账号已创建, 请使用 {rootUser.UserName} 和 空 密码 来登录后台系统并修改密码.");
        }

    }
    
    //use static files
    app.UseFileServer();
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e.Message);
    logger.Error(e.StackTrace);
}
logger.Info("Application Ended");

