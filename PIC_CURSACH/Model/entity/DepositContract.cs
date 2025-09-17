using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity
{
    [Table("depositcontracts")]
    public class DepositContract
    {
        [Key]
        [Column("contractid")]
        public int ContractId { get; set; }

        [Column("clientid")]
        public int ClientId { get; set; }

        [Column("typeid")]
        public int TypeId { get; set; }

        [Column("employeeid")]
        public int EmployeeId { get; set; }

        [Column("branchid")]
        public int BranchId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("startdate")]
        public DateTime StartDate { get; set; }

        [Column("enddate")]
        public DateTime EndDate { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "ACTIVE";

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        [ForeignKey("TypeId")]
        public virtual DepositType DepositType { get; set; } = null!;

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        public virtual ICollection<DepositOperation> DepositOperations { get; set; } = new List<DepositOperation>();
    }
}
