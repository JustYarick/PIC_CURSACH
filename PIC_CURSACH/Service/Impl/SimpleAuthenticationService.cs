using PIC_CURSACH.Model.core;
using PIC_CURSACH.Service.Interfaces;
using User = PIC_CURSACH.Model.core.User;
using UserRole = PIC_CURSACH.Model.enums.UserRole;

namespace PIC_CURSACH.Service.Impl;

public class SimpleAuthenticationService : IAuthenticationService
{
    private readonly List<User> _testUsers = new List<User>
    {
        new User { Username = "admin", Password = "admin", Role = UserRole.Admin },
        new User { Username = "manager", Password = "manager", Role = UserRole.Manager },
        new User { Username = "operator", Password = "operator", Role = UserRole.Operator },
        new User { Username = "viewer", Password = "viewer", Role = UserRole.Viewer }
    };

    public AuthenticationResult Authenticate(string username, string password)
    {
        var user = _testUsers.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user != null)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user
            };
        }

        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = "Неверный логин или пароль"
        };
    }
}