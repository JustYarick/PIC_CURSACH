using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity
{
    [Table("deposittypes")]
    public class DepositType
    {
        [Key]
        [Column("typeid")]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("interestrate")]
        [Range(0, 100)]
        public decimal InterestRate { get; set; }

        [Column("minamount")]
        public decimal MinAmount { get; set; }

        [Column("termdays")]
        public int TermDays { get; set; }

        public virtual ICollection<DepositContract> DepositContracts { get; set; } = new List<DepositContract>();
    }
}
