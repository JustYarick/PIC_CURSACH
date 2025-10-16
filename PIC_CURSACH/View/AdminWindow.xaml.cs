using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.ViewModel;

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
}