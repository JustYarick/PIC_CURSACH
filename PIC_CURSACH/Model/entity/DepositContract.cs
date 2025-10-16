using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("deposit_contracts")]
public class DepositContract
{
    [Key]
    [Column("contract_id")]
    public int ContractId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "ACTIVE";

    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;

    [ForeignKey("TypeId")]
    public DepositType DepositType { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;

    [ForeignKey("BranchId")]
    public Branch Branch { get; set; } = null!;
}
