using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("audit_log")]
public class AuditLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("table_name")]
    [MaxLength(50)]
    public string? TableName { get; set; }

    [Column("operation")]
    [MaxLength(10)]
    public string? Operation { get; set; }

    [Column("old_values")]
    public string? OldValues { get; set; }

    [Column("new_values")]
    public string? NewValues { get; set; }

    [Column("changed_by")]
    [MaxLength(50)]
    public string? ChangedBy { get; set; }

    [Column("changed_at")]
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}