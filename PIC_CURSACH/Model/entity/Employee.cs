using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("employees")]
public class Employee
{
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("position")]
    public string Position { get; set; } = string.Empty;

    public virtual ICollection<DepositContract> DepositContracts { get; set; } = new List<DepositContract>();
}