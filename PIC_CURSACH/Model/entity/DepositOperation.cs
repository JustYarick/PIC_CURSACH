using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity
{
    [Table("depositoperations")]
    public class DepositOperation
    {
        [Key]
        [Column("operationid")]
        public int OperationId { get; set; }

        [Column("contractid")]
        public int ContractId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("operationtype")]
        public string OperationType { get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("operationdate")]
        public DateTime OperationDate { get; set; } = DateTime.Now;

        [ForeignKey("ContractId")]
        public virtual DepositContract DepositContract { get; set; } = null!;
    }
}
