using MCServerWebManagerBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace MCServerWebManagerBackend.Data
{
    public interface IContext
    {
        DbSet<McServer> McServers { get; set; }

        DbSet<Token> Tokens { get; set; }

        DbSet<User> Users { get; set; }

        DbContext Ctx { get;}

    }


    public class Context :DbContext, IContext
    {
        private ILogger<Context> _logger;
        
        public Context(DbContextOptions<Context> options, ILogger<Context> logger) : base(options)
        {
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo( (x) => _logger.LogDebug(x));
        }

        public DbSet<McServer> McServers { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<User> Users { get; set; }

        public DbContext Ctx
        {
            get => this;
        }
    
    }    
    
}
