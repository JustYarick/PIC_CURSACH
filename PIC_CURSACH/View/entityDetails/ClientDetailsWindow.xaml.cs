using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.ViewModel.entityDetail;

namespace PIC_CURSACH.View.entityDetails
{
    public partial class ClientDetailsWindow : Window
    {
        private readonly IClientService _clientService;
        private Client _client;
        private ClientViewModel _viewModel;

        public ClientDetailsWindow(Client selectedClient)
        {
            InitializeComponent();

            IServiceProvider serviceProvider = App.CurrentServiceConfigurator.Services;
            _clientService = serviceProvider.GetRequiredService<IClientService>();

            Loaded += async (s, e) => await LoadClientAsync(selectedClient.ClientId);
        }

        private async Task LoadClientAsync(int clientId)
        {
            try
            {
                _client = await _clientService.GetByIdAsync(clientId);

                if (_client == null)
                {
                    MessageBox.Show("Клиент не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                var contracts = _client.DepositContracts?.ToList() ?? new List<DepositContract>();

                _viewModel = new ClientViewModel
                {
                    ClientId = _client.ClientId,
                    FirstName = _client.FirstName,
                    LastName = _client.LastName,
                    Passport = _client.Passport,
                    Phone = _client.Phone ?? string.Empty,
                    Email = _client.Email ?? string.Empty,
                    DepositContracts = new ObservableCollection<DepositContract>(contracts)
                };

                DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveButton.IsEnabled = false;

                // Проверяем обязательные поля
                if (string.IsNullOrWhiteSpace(_viewModel.FirstName))
                {
                    MessageBox.Show("Введите имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(_viewModel.LastName))
                {
                    MessageBox.Show("Введите фамилию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(_viewModel.Passport))
                {
                    MessageBox.Show("Введите паспорт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем поля
                _client.FirstName = _viewModel.FirstName;
                _client.LastName = _viewModel.LastName;
                _client.Passport = _viewModel.Passport;
                _client.Phone = string.IsNullOrWhiteSpace(_viewModel.Phone) ? null : _viewModel.Phone;
                _client.Email = string.IsNullOrWhiteSpace(_viewModel.Email) ? null : _viewModel.Email;

                // Сохраняем
                await _clientService.UpdateAsync(_client);

                MessageBox.Show("Изменения успешно сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Ошибка при сохранении:\n\n{ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nВнутренняя ошибка:\n{ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
