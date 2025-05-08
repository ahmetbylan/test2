using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BUygulamasi.Models
{
    public class KullaniciRol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KullaniciRolID { get; set; }

        [Required]
        public int KullaniciID { get; set; }

        [Required]
        public int RolID { get; set; }

        [ForeignKey("KullaniciID")]
        public Kullanici Kullanici { get; set; } = null!;

        [ForeignKey("RolID")]
        public Rol Rol { get; set; } = null!;
    }
}