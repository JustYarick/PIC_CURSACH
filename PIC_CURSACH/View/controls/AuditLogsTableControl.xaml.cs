using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.ViewModel.controls;

namespace PIC_CURSACH.View.controls;

public partial class AuditLogsTableControl : UserControl
{
    public AuditLogsTableViewModel? ViewModel { get; private set; }
    private bool _isLoaded = false;

    public AuditLogsTableControl()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isLoaded) return;
        _isLoaded = true;

        var serviceProvider = App.CurrentServiceConfigurator?.Services;
        if (serviceProvider != null)
        {
            ViewModel = serviceProvider.GetRequiredService<AuditLogsTableViewModel>();
            DataContext = ViewModel;

            await ViewModel.LoadDataAsync();
        }
    }
}