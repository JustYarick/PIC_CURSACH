using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("documents")]
public class Document
{
    [Key]
    [Column("document_id")]
    public int DocumentId { get; set; }

    [Column("contract_id")]
    public int? ContractId { get; set; }

    [Column("document_type")]
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;

    [Column("file_path")]
    [MaxLength(500)]
    public string? FilePath { get; set; }

    [ForeignKey(nameof(ContractId))]
    public virtual DepositContract? Contract { get; set; }
}