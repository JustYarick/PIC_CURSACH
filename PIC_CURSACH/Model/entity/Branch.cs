using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("branches")]
public class Branch
{
    [Key] [Column("branchid")] public int BranchId { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("address")] public string? Address { get; set; }
}
