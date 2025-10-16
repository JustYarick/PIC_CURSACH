using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("branches")]
public class Branch
{
    [Key]
    [Column("branch_id")]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("address")]
    public string? Address { get; set; }

    public virtual ICollection<DepositContract> DepositContracts { get; set; } = new List<DepositContract>();
}