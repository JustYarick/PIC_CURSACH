using PIC_CURSACH.Model.core;

namespace PIC_CURSACH.Service.Interfaces;

public interface IAuthenticationService
{
    AuthenticationResult Authenticate(string username, string password);
}