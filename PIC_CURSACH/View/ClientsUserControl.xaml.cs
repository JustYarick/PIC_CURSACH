using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ClientsViewModel = PIC_CURSACH.ViewModel.ClientsViewModel;

namespace PIC_CURSACH.View
{
    public partial class ClientsUserControl : UserControl
    {
        public ClientsViewModel ViewModel { get; }

        public ClientsUserControl()
        {
            InitializeComponent();

            var app = Application.Current as App;
            var serviceConfigurator = app?.CurrentServiceConfigurator;
            if (serviceConfigurator == null)
            {
                MessageBox.Show("ServiceConfigurator не инициализирован.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ViewModel = serviceConfigurator.Services.GetRequiredService<ClientsViewModel>();
            DataContext = ViewModel;

            _ = ViewModel.LoadClientsCommand.ExecuteAsync(null);
        }
    }
}