using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.enums;
using PIC_CURSACH.Service.Impl;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;

namespace PIC_CURSACH.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService =
        App.CurrentServiceConfigurator.Services.GetRequiredService<IAuthenticationService>();

    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string errorMessage = "";

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = string.Empty;

        AuthenticationResult result = _authService.Authenticate(Username, Password);

        if (result.IsSuccess && result.User != null)
        {
            Application.Current.MainWindow.Hide();
            Window win = result.User.Role switch
            {
                UserRole.Admin => new AdminWindow(),
                UserRole.Manager => new ManagerWindow(),
                UserRole.Operator => new RoleWindow("Операционист"),
                UserRole.Viewer => new RoleWindow("Наблюдатель"),
                _ => new RoleWindow("Неизвестно")
            };
            win.ShowDialog();
            Application.Current.MainWindow.Show();
            Password = "";
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
    }
}