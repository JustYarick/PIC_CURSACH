using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;
using PIC_CURSACH.View.entityDetails;

namespace PIC_CURSACH.ViewModel.controls;

public partial class ClientsTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Client> clients = new();
    [ObservableProperty] private Client? selectedClient;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IClientService _clientService;

    public ClientsTableViewModel(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _clientService.GetAllAsync();
        Clients = new ObservableCollection<Client>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "Имя", Placeholder = "Введите имя клиента", IsRequired = true, PropertyName = "FirstName" },
            new() { Label = "Фамилия", Placeholder = "Введите фамилию клиента", IsRequired = true, PropertyName = "LastName" },
            new() { Label = "Паспорт", Placeholder = "Введите номер паспорта", IsRequired = true, PropertyName = "Passport" },
            new() { Label = "Телефон", Placeholder = "Введите номер телефона", PropertyName = "Phone" },
            new() { Label = "Email", Placeholder = "Введите email адрес", PropertyName = "Email" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление клиента", "Добавить", fields, SaveNewClientAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedClient == null) return;

        var fields = new List<FormField>
        {
            new() { Label = "Имя", IsRequired = true, PropertyName = "FirstName", Value = SelectedClient.FirstName },
            new() { Label = "Фамилия", IsRequired = true, PropertyName = "LastName", Value = SelectedClient.LastName },
            new() { Label = "Паспорт", IsRequired = true, PropertyName = "Passport", Value = SelectedClient.Passport, IsEnabled = false },
            new() { Label = "Телефон", PropertyName = "Phone", Value = SelectedClient.Phone ?? "" },
            new() { Label = "Email", PropertyName = "Email", Value = SelectedClient.Email ?? "" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование клиента", "Сохранить", fields,
            values => SaveExistingClientAsync(SelectedClient.ClientId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteClient()
    {
        if (SelectedClient == null) return;

        var result = MessageBox.Show($"Удалить клиента {SelectedClient.FirstName} {SelectedClient.LastName}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _clientService.DeleteAsync(SelectedClient.ClientId);
            Clients.Remove(SelectedClient);
            MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public void OpenClientDetails()
    {
        if (SelectedClient == null) return;

        var detailsWindow = new ClientDetailsWindow(SelectedClient);
        detailsWindow.ShowDialog();
    }

    private async Task<bool> SaveNewClientAsync(Dictionary<string, string> values)
    {
        try
        {
            var exists = await _clientService.ExistsByPassportAsync(values["Passport"]);
            if (exists)
            {
                MessageBox.Show($"Клиент с паспортом {values["Passport"]} уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var client = new Client
            {
                FirstName = values["FirstName"],
                LastName = values["LastName"],
                Passport = values["Passport"],
                Phone = string.IsNullOrEmpty(values.GetValueOrDefault("Phone")) ? null : values["Phone"],
                Email = string.IsNullOrEmpty(values.GetValueOrDefault("Email")) ? null : values["Email"]
            };

            await _clientService.AddAsync(client);
            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException?.Message ?? ex.Message;
            MessageBox.Show($"Ошибка при добавлении клиента: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingClientAsync(int clientId, Dictionary<string, string> values)
    {
        try
        {
            var client = await _clientService.GetByIdAsync(clientId);
            if (client == null)
            {
                MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            client.FirstName = values["FirstName"];
            client.LastName = values["LastName"];
            client.Phone = string.IsNullOrEmpty(values.GetValueOrDefault("Phone")) ? null : values["Phone"];
            client.Email = string.IsNullOrEmpty(values.GetValueOrDefault("Email")) ? null : values["Email"];

            await _clientService.UpdateAsync(client);
            MessageBox.Show("Клиент успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}