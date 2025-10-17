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

    // Navigation properties
    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("TypeId")]
    public virtual DepositType DepositType { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; } = null!;

    // Навигация по операциям
    public virtual ICollection<DepositOperation> DepositOperations { get; set; } = new List<DepositOperation>();

    // Навигация по документам
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public void AddDepositOperation(DepositOperation operation)
    {
        operation.DepositContract = this;
        DepositOperations.Add(operation);
    }

    public void AddDocument(Document document)
    {
        document.Contract = this;
        Documents.Add(document);
    }

    [NotMapped]
    public string ClientDisplay => Client != null ? $"{Client.FirstName} {Client.LastName} | {Client.Passport}" : "";

    [NotMapped]
    public string EmployeeDisplay => Employee != null ? $"{Employee.FirstName} {Employee.LastName}, {Employee.Position}" : "";

    [NotMapped]
    public string BranchDisplay => Branch != null ? $"{Branch.Name}, {Branch.Address}" : "";

    [NotMapped]
    public string DepositTypeDisplay => DepositType != null ? $"{DepositType.Name} ({DepositType.InterestRate}% на {DepositType.TermDays} дн., мин. {DepositType.MinAmount})" : "";

    [NotMapped]
    public int DocumentsCount => Documents?.Count ?? 0;

    [NotMapped]
    public string DocumentsDisplay => Documents != null && Documents.Any()
        ? string.Join(", ", Documents.Select(d => d.DocumentType))
        : "Нет документов";
}
