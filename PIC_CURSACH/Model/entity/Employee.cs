using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIC_CURSACH.Model.entity;

[Table("employees")]
public class Employee
{
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("position")]
    public string Position { get; set; }
}
