using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("deposit_types")]
public class DepositType
{
    [Key]
    [Column("type_id")]
    public int TypeId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("interest_rate")]
    public decimal InterestRate { get; set; }

    [Column("min_amount")]
    public decimal MinAmount { get; set; }

    [Column("term_days")]
    public int TermDays { get; set; }

    public virtual ICollection<DepositContract> DepositContracts { get; set; } = new List<DepositContract>();
}