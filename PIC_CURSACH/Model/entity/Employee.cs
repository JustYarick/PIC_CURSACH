using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("employees")]
public class Employee
{
    [Key] [Column("employeeid")] public int EmployeeId { get; set; }
    [Column("firstname")] public string FirstName { get; set; }
    [Column("position")] public string Position { get; set; }
}
