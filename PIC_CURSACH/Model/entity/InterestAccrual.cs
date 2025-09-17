using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity
{
    [Table("interestaccruals")]
    public class InterestAccrual
    {
        [Key]
        [Column("accrualid")]
        public int AccrualId { get; set; }

        [Column("contractid")]
        public int ContractId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("accrualdate")]
        public DateTime AccrualDate { get; set; }

        [ForeignKey("ContractId")]
        public virtual DepositContract DepositContract { get; set; } = null!;
    }
}
