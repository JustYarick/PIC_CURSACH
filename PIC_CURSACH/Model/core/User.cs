using PIC_CURSACH.Model.entity;
using UserRole = PIC_CURSACH.Model.enums.UserRole;

namespace PIC_CURSACH.Model.core;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string RoleName => Role.ToString();
}