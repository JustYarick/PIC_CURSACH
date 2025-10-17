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
    public partial class EmployeeDetailsWindow : Window
    {
        private readonly IEmployeeService _employeeService;
        private Employee _employee;
        private EmployeeViewModel _viewModel;

        public EmployeeDetailsWindow(Employee selectedEmployee)
        {
            InitializeComponent();

            IServiceProvider serviceProvider = App.CurrentServiceConfigurator.Services;
            _employeeService = serviceProvider.GetRequiredService<IEmployeeService>();

            Loaded += async (s, e) => await LoadEmployeeAsync(selectedEmployee.EmployeeId);
        }

        private async Task LoadEmployeeAsync(int employeeId)
        {
            try
            {
                _employee = await _employeeService.GetByIdAsync(employeeId);

                if (_employee == null)
                {
                    MessageBox.Show("Сотрудник не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                var contracts = _employee.DepositContracts?.ToList() ?? new List<DepositContract>();

                _viewModel = new EmployeeViewModel
                {
                    EmployeeId = _employee.EmployeeId,
                    FirstName = _employee.FirstName,
                    LastName = _employee.LastName,
                    Position = _employee.Position,
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
                    MessageBox.Show("Введите имя!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(_viewModel.LastName))
                {
                    MessageBox.Show("Введите фамилию!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(_viewModel.Position))
                {
                    MessageBox.Show("Введите должность!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем поля
                _employee.FirstName = _viewModel.FirstName;
                _employee.LastName = _viewModel.LastName;
                _employee.Position = _viewModel.Position;

                // Сохраняем
                await _employeeService.UpdateAsync(_employee);

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
