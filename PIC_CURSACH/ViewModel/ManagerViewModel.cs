using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.ViewModel;

public partial class ManagerViewModel : ObservableObject
{
    [ObservableProperty] private bool isListView = true;
    [ObservableProperty] private bool isDetailView = false;
    [ObservableProperty] private bool isClientDetail = false;
    [ObservableProperty] private bool isContractDetail = false;
    [ObservableProperty] private string currentViewTitle = "Панель менеджера";

    [ObservableProperty] private ObservableCollection<Client> clients = new();
    [ObservableProperty] private Client? selectedClient;
    [ObservableProperty] private ObservableCollection<DepositContract> clientContracts = new();

    [ObservableProperty] private ObservableCollection<DepositContract> depositContracts = new();
    [ObservableProperty] private DepositContract? selectedDepositContract;
    [ObservableProperty] private ObservableCollection<DepositOperation> contractOperations = new();

    private readonly IClientService _clientService;
    private readonly IDepositContractService _contractService;
    private readonly IDepositOperationService _operationService;

    [RelayCommand]
    public async Task ShowClientDetails(Client client)
    {
        SelectedClient = client;
        IsListView = false;
        IsDetailView = true;
        IsClientDetail = true;
        IsContractDetail = false;
        CurrentViewTitle = $"Клиент: {client.FirstName} {client.LastName}";

        // Загрузить связанные договоры
        var contracts = await _contractService.GetAllAsync();
        ClientContracts = new(contracts.Where(c => c.ClientId == client.ClientId));
    }

    [RelayCommand]
    public async Task ShowContractDetails(DepositContract contract)
    {
        SelectedDepositContract = contract;
        IsListView = false;
        IsDetailView = true;
        IsClientDetail = false;
        IsContractDetail = true;
        CurrentViewTitle = $"Договор #{contract.ContractId}";

        // Загрузить связанные операции
        var operations = await _operationService.GetAllAsync();
        ContractOperations = new(operations.Where(o => o.ContractId == contract.ContractId));
    }

    [RelayCommand]
    public void BackToList()
    {
        IsListView = true;
        IsDetailView = false;
        IsClientDetail = false;
        IsContractDetail = false;
        CurrentViewTitle = "Панель менеджера";
    }
}
