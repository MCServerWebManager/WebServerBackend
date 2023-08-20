using System.Text;

namespace MCServerWebManagerBackend.Data;

public static class Extensions
{
    public static string GenerateToken(int len = 128)
    {
        //token 用大小写字母组合
        var str = new StringBuilder();
        var random = new Random(Guid.NewGuid().GetHashCode());
        
        for (int i = 0; i < len; i++)
        {
            var c = 'A' + (random.Next() % 26);
            var lowerCaseMask = (random.Next() % 2) << 5;
            str.Append((char)(c | lowerCaseMask));
        }

        return str.ToString();
    }

}