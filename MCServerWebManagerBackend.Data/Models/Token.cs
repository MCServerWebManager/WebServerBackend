using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCServerWebManagerBackend.Data.Models
{
    [Table("Tokens")]
    public class Token
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(128)] 
        public string TokenStr { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Guid User { get; set; }
    }    
    
}

