using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Service.Impl;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;
using ClientsViewModel = PIC_CURSACH.ViewModel.ClientsViewModel;

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
        // DbContext для PostgreSQL (Entity Framework Core)
        services.AddDbContext<DepositContext>(options =>
            options.UseNpgsql("Host=localhost;Database=pic_cursach;Username=root;Password=root"));

        // Services
        services.AddTransient<IClientService, ClientService>();

        // ViewModels
        services.AddTransient<ClientsViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<ClientsUserControl>();
    }
}
