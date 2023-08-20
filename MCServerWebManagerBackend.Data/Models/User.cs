using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCServerWebManagerBackend.Data.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

    }    
    
}
