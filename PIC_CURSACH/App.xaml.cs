using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Configuration;

namespace PIC_CURSACH;

public partial class App : Application
{
    public static ServiceConfigurator CurrentServiceConfigurator { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceConfigurator = new ServiceConfigurator();
        CurrentServiceConfigurator = serviceConfigurator;


        var mainWindow = CurrentServiceConfigurator.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}