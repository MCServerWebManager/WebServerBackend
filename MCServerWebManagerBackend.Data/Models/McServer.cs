using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCServerWebManagerBackend.Data.Models
{
    [Table("McServers")]
    public class McServer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(128)]
        public string ServerAddress { get; set; } = "";

        [MaxLength(128)] 
        public string ServerKey { get; set; } = "";

        public Guid UserGuid { get; set; }
    }    
    
};
