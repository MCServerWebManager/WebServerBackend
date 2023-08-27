using MCServerWebManagerBackend.ConfigModels;

namespace MCServerWebManagerBackend
{
    namespace ConfigModels
    {
        public class Login
        {
            //token 过期时长
            public long TokenExpireTime { get; set; } = 0;
        }

    }
    
    public class AppSettings
    {
        public Login Login { get; set; } = new();
    }   
}