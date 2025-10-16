using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("deposit_operations")]
public class DepositOperation
{
    [Key]
    [Column("operation_id")]
    public int OperationId { get; set; }

    [Column("contract_id")]
    public int ContractId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("operation_type")]
    public string OperationType { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("operation_date")]
    public DateTime OperationDate { get; set; } = DateTime.Now;

    [ForeignKey("ContractId")]
    public DepositContract DepositContract { get; set; } = null!;
}
