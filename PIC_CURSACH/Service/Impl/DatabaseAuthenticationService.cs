using Npgsql;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Service.Interfaces;
using User = PIC_CURSACH.Model.core.User;
using UserRole = PIC_CURSACH.Model.enums.UserRole;

namespace PIC_CURSACH.Service.Impl;

public class DatabaseAuthenticationService : IAuthenticationService
{
    private readonly string _host = "localhost";
    private readonly string _database = "pic_cursach";
    private readonly int _port = 5432;

    public AuthenticationResult Authenticate(string username, string password)
    {
        var result = new AuthenticationResult();

        string connStr = $"Host={_host};Port={_port};Database={_database};Username={username};Password={password}";

        try
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            var cmdRoles = new NpgsqlCommand(@"
                SELECT r.rolname
                FROM pg_roles r
                JOIN pg_auth_members m ON r.oid = m.roleid
                JOIN pg_roles u ON u.oid = m.member
                WHERE u.rolname = current_user;
            ", conn);

            List<string> roles = new List<string>();
            using (var reader = cmdRoles.ExecuteReader())
            {
                while (reader.Read())
                {
                    roles.Add(reader.GetString(0));
                }
            }

            string currentUser;
            using (var cmdUser = new NpgsqlCommand("SELECT current_user;", conn))
            {
                currentUser = (string)cmdUser.ExecuteScalar()!;
            }

            if (roles.Count == 0)
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "У пользователя нет ролей в базе."
                };

            UserRole appRole = MapPgRoleToAppRole(roles);

            result.IsSuccess = true;
            result.User = new User
            {
                Username = currentUser,
                Role = appRole
            };

            return result;
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = $"Ошибка подключения или авторизации: {ex.Message}"
            };
        }
    }

    private UserRole MapPgRoleToAppRole(List<string> pgRoles)
    {
        if (pgRoles.Contains("admin_role"))
            return UserRole.Admin;
        if (pgRoles.Contains("manager_role"))
            return UserRole.Manager;
        if (pgRoles.Contains("operator_role"))
            return UserRole.Operator;
        if (pgRoles.Contains("viewer_role"))
            return UserRole.Viewer;

        return UserRole.Viewer;
    }
}