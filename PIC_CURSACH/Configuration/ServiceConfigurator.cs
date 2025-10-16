using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Service.Impl;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Configuration;

public class ServiceConfigurator
{
    public IServiceProvider Services { get; private set; } = ConfigureDiServices();

    private static IServiceProvider ConfigureDiServices()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        return serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Views
        services.AddTransient<MainWindow>();

        // Services
        services.AddSingleton<IAuthenticationService, DatabaseAuthenticationService>();
    }
}
