using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BUygulamasi.Models
{
    [Table("Roller")]
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolID { get; set; }

        [Required]
        [StringLength(50)]
        public string RolAdi { get; set; } = null!;

        // Navigation Property
        public virtual ICollection<KullaniciRol> KullaniciRolleri { get; set; } = new List<KullaniciRol>();
    }
}