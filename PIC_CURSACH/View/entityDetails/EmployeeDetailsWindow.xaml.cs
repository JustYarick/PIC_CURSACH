using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
        private readonly bool _canEdit;

        public EmployeeDetailsWindow(Employee selectedEmployee, bool canEdit = false)
        {
            InitializeComponent();
            _canEdit = canEdit;

            // Изменяем заголовок окна
            Title = canEdit ? "Редактирование сотрудника" : "Просмотр сотрудника";

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
                    DepositContracts = new ObservableCollection<DepositContract>(contracts),
                    IsEditable = _canEdit,
                    CloseButtonText = _canEdit ? "Отмена" : "Закрыть"
                };

                DataContext = _viewModel;

                // Настраиваем UI в зависимости от прав
                ApplyEditMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void ApplyEditMode()
        {
            if (!_canEdit)
            {
                // Скрываем кнопку сохранения
                SaveButton.Visibility = Visibility.Collapsed;

                // Делаем все текстовые поля только для чтения
                foreach (var textBox in FindVisualChildren<Wpf.Ui.Controls.TextBox>(this))
                {
                    textBox.IsReadOnly = true;
                }
            }
        }

        // Вспомогательный метод для поиска дочерних элементов
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T t)
                {
                    yield return t;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return; // Дополнительная защита

            try
            {
                SaveButton.IsEnabled = false;

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

                _employee.FirstName = _viewModel.FirstName;
                _employee.LastName = _viewModel.LastName;
                _employee.Position = _viewModel.Position;

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
            if (!_canEdit)
            {
                Close();
                return;
            }

            if (MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
