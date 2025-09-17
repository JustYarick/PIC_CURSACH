using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.ViewModel
{
    public partial class ClientsViewModel : ObservableObject
    {
        private readonly IClientService _clientService;

        [ObservableProperty]
        private ObservableCollection<Client> clients = new();

        [ObservableProperty]
        private Client? selectedClient;

        [ObservableProperty]
        private Client newClient = new Client();

        public ClientsViewModel(IClientService clientService)
        {
            _clientService = clientService;
        }

        [RelayCommand]
        private async Task LoadClients()
        {
            var data = await _clientService.GetAllAsync();
            Clients = new ObservableCollection<Client>(data);
        }

        [RelayCommand]
        private async Task AddClient()
        {
            if (string.IsNullOrWhiteSpace(NewClient.Passport))
            {
                MessageBox.Show("Поле 'Паспорт' обязательно для заполнения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var exists = await _clientService.ExistsByPassportAsync(NewClient.Passport);
            if (exists)
            {
                MessageBox.Show($"Клиент с паспортом {NewClient.Passport} уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _clientService.AddAsync(NewClient);
            await LoadClients();

            NewClient = new Client();
        }

        [RelayCommand]
        private async Task EditClient()
        {
            if (SelectedClient == null) return;

            await _clientService.UpdateAsync(SelectedClient);
            MessageBox.Show("Клиент обновлен!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
            }
        }
    }
}
