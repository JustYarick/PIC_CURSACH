using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Service.Impl;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.ViewModel;
using PIC_CURSACH.ViewModel.controls;

namespace PIC_CURSACH.Configuration;

public class ServiceConfigurator
{
    public IServiceProvider Services { get; private set; } = ConfigureDiServices();

    private static ServiceCollection serviceCollection = new();

    private static IServiceProvider ConfigureDiServices()
    {
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

    public void RegisterDbContext(string username, string password)
    {
        string connStr = $"Host=localhost;Database=pic_cursach;Username={username};Password={password}";

        serviceCollection.AddDbContext<DepositContext>(options =>
            options.UseLazyLoadingProxies()
                .UseNpgsql(connStr));

        IServiceCollection services =  serviceCollection;

        // Services require DB context
        services.AddSingleton<IClientService, ClientService>();
        services.AddSingleton<IEmployeeService, EmployeeService>();
        services.AddSingleton<IDepositContractService, DepositContractService>();
        services.AddSingleton<IDepositOperationService, DepositOperationService>();
        services.AddSingleton<IDepositTypeService, DepositTypeService>();
        services.AddSingleton<IBranchService, BranchService>();
        services.AddSingleton<IDocumentService, DocumentService>();
        services.AddSingleton<IAuditLogService, AuditLogService>();

        services.AddTransient<ClientsTableViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<ManagerViewModel>();
        services.AddTransient<EmployeesTableViewModel>();
        services.AddTransient<DepositOperationsTableViewModel>();
        services.AddTransient<DepositTypesTableViewModel>();
        services.AddTransient<DepositContractsTableViewModel>();
        services.AddScoped<DocumentsTableViewModel>();
        services.AddScoped<AuditLogsTableViewModel>();


        Services = serviceCollection.BuildServiceProvider();
    }
}
