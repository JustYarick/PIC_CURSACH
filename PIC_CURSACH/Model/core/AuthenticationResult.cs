namespace PIC_CURSACH.Model.core;

public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public User? User { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}