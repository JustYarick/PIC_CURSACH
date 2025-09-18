using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;

namespace PIC_CURSACH.ViewModel;

public partial class ClientsViewModel : ObservableObject
{
    private readonly IClientService _clientService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Client? selectedClient;

    public ClientsViewModel(IClientService clientService, IServiceProvider serviceProvider)
    {
        _clientService = clientService;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task LoadClients()
    {
        var data = await _clientService.GetAllAsync();
        Clients = new ObservableCollection<Client>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        // Определяем поля для формы добавления
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
            await LoadClients();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedClient == null) return;

        // Определяем поля для формы редактирования с текущими значениями
        var fields = new List<FormField>
        {
            new() { Label = "Имя", Placeholder = "Введите имя клиента", IsRequired = true, PropertyName = "FirstName", Value = SelectedClient.FirstName },
            new() { Label = "Фамилия", Placeholder = "Введите фамилию клиента", IsRequired = true, PropertyName = "LastName", Value = SelectedClient.LastName },
            new() { Label = "Паспорт", Placeholder = "Номер паспорта", IsRequired = true, PropertyName = "Passport", Value = SelectedClient.Passport, IsEnabled = false },
            new() { Label = "Телефон", Placeholder = "Введите номер телефона", PropertyName = "Phone", Value = SelectedClient.Phone ?? "" },
            new() { Label = "Email", Placeholder = "Введите email адрес", PropertyName = "Email", Value = SelectedClient.Email ?? "" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование клиента", "Сохранить", fields,
            values => SaveExistingClientAsync(SelectedClient.ClientId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadClients();
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
            await LoadClients();
            MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Метод сохранения нового клиента
    private async Task<bool> SaveNewClientAsync(Dictionary<string, string> values)
    {
        try
        {
            // Проверка уникальности паспорта
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
            await LoadClients();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    // Метод сохранения изменений существующего клиента
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
            // Паспорт не обновляем - он заблокирован при редактировании
            client.Phone = string.IsNullOrEmpty(values.GetValueOrDefault("Phone")) ? null : values["Phone"];
            client.Email = string.IsNullOrEmpty(values.GetValueOrDefault("Email")) ? null : values["Email"];

            await _clientService.UpdateAsync(client);
            MessageBox.Show("Клиент успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}