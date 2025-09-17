using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity
{
    [Table("userroles")]
    public class UserRole
    {
        [Key]
        [Column("roleid")]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("rolename")]
        public string RoleName { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
