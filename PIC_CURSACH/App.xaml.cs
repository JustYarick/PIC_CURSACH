using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Configuration;

namespace PIC_CURSACH;

public partial class App : Application
{
    public ServiceConfigurator CurrentServiceConfigurator { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceConfigurator = new ServiceConfigurator();
        CurrentServiceConfigurator = serviceConfigurator;

        var dbContext = CurrentServiceConfigurator.Services.GetRequiredService<DepositContext>();

        try
        {
            bool canConnect = await dbContext.Database.CanConnectAsync();

            if (canConnect)
            {
                MessageBox.Show("Подключение к базе данных успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к базе данных!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения к БД:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var mainWindow = CurrentServiceConfigurator.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}