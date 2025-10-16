using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.ViewModel;

using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace PIC_CURSACH.View;

public partial class AdminWindow : Window
{
    public AdminViewModel ViewModel { get; }

    public AdminWindow()
    {
        InitializeComponent();

        var serviceConfigurator = App.CurrentServiceConfigurator;
        if (serviceConfigurator == null)
        {
            MessageBox.Show("ServiceConfigurator не инициализирован.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ViewModel = serviceConfigurator.Services.GetRequiredService<AdminViewModel>();
        DataContext = ViewModel;

        Loaded += async (s, e) => await ViewModel.InitializeAsync();
    }

    private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AdminViewModel vm && vm.SelectedClient != null)
        {
            ViewModel.OpenClientDetails();
        }
    }

    private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AdminViewModel vm && vm.SelectedEmployee != null)
        {
            ViewModel.OpenEmployeeDetails();
        }
    }

    private void DepositTypeDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AdminViewModel vm && vm.SelectedDepositType != null)
        {
            ViewModel.OpenDepositTypeDetails();
        }
    }

    private void DepositContractDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AdminViewModel vm && vm.SelectedDepositContract != null)
        {
            ViewModel.OpenDepositContractDetails();
        }
    }
}
