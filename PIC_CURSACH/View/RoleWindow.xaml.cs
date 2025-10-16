using System.Windows;

namespace PIC_CURSACH.View;

public partial class RoleWindow : Window
{
    public RoleWindow(string role)
    {
        InitializeComponent();
        RoleText.Text = $"Роль: {role}";
    }

    private void Button_Logout(object sender, RoutedEventArgs e)
    {
        Close();
    }
}