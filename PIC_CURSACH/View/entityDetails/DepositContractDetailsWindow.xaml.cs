using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.ViewModel.entityDetail;

namespace PIC_CURSACH.View.entityDetails
{
    public partial class DepositContractDetailsWindow : Window
    {
        private readonly IDepositContractService _depositService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly IBranchService _branchService;
        private readonly IDepositTypeService _depositTypeService;

        private DepositContract _contract;
        private DepositContractViewModel _viewModel;
        private readonly bool _canEdit;

        public DepositContractDetailsWindow(DepositContract selectedContract, bool canEdit = false)
        {
            InitializeComponent();
            _canEdit = canEdit;

            // Изменяем заголовок окна
            Title = canEdit ? "Редактирование депозитного договора" : "Просмотр депозитного договора";

            IServiceProvider serviceProvider = App.CurrentServiceConfigurator.Services;

            _depositService = serviceProvider.GetRequiredService<IDepositContractService>();
            _clientService = serviceProvider.GetRequiredService<IClientService>();
            _employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
            _branchService = serviceProvider.GetRequiredService<IBranchService>();
            _depositTypeService = serviceProvider.GetRequiredService<IDepositTypeService>();

            Loaded += async (s, e) => await LoadContractAsync(selectedContract.ContractId);
        }

        private async Task LoadContractAsync(int contractId)
        {
            try
            {
                _contract = await _depositService.GetByIdAsync(contractId);

                if (_contract == null)
                {
                    MessageBox.Show("Депозит не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                var documents = _contract.Documents?.ToList() ?? new List<Document>();
                var operations = _contract.DepositOperations?.ToList() ?? new List<DepositOperation>();

                var clients = await _clientService.GetAllAsync();
                var employees = await _employeeService.GetAllAsync();
                var branches = await _branchService.GetAllAsync();
                var depositTypes = await _depositTypeService.GetAllAsync();

                _viewModel = new DepositContractViewModel
                {
                    ContractId = _contract.ContractId,
                    Amount = _contract.Amount,
                    StartDate = _contract.StartDate,
                    EndDate = _contract.EndDate,
                    Status = _contract.Status,

                    AvailableClients = new ObservableCollection<Client>(clients),
                    SelectedClientId = _contract.ClientId,

                    AvailableEmployees = new ObservableCollection<Employee>(employees),
                    SelectedEmployeeId = _contract.EmployeeId,

                    AvailableBranches = new ObservableCollection<Branch>(branches),
                    SelectedBranchId = _contract.BranchId,

                    AvailableDepositTypes = new ObservableCollection<DepositType>(depositTypes),
                    SelectedDepositTypeId = _contract.TypeId,

                    Documents = new ObservableCollection<Document>(documents),
                    DepositOperations = new ObservableCollection<DepositOperation>(operations),

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

                // Делаем все NumberBox только для чтения
                foreach (var numberBox in FindVisualChildren<Wpf.Ui.Controls.NumberBox>(this))
                {
                    numberBox.IsReadOnly = true;
                }

                // Делаем DatePicker недоступными
                foreach (var datePicker in FindVisualChildren<DatePicker>(this))
                {
                    datePicker.IsEnabled = false;
                }

                // Делаем ComboBox недоступными
                foreach (var comboBox in FindVisualChildren<ComboBox>(this))
                {
                    comboBox.IsEnabled = false;
                }

                // Делаем DataGrid только для чтения
                DocumentsDataGrid.IsReadOnly = true;
                OperationsDataGrid.IsReadOnly = true;

                // Скрываем кнопки добавления/удаления
                foreach (var button in FindVisualChildren<Wpf.Ui.Controls.Button>(this))
                {
                    if (button.Content?.ToString()?.Contains("Добавить") == true ||
                        button.Content?.ToString()?.Contains("Удалить") == true)
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
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

                // Проверяем обязательные поля
                if (!_viewModel.SelectedClientId.HasValue)
                {
                    MessageBox.Show("Выберите клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!_viewModel.SelectedEmployeeId.HasValue)
                {
                    MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!_viewModel.SelectedBranchId.HasValue)
                {
                    MessageBox.Show("Выберите филиал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!_viewModel.SelectedDepositTypeId.HasValue)
                {
                    MessageBox.Show("Выберите тип депозита!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем основные поля
                _contract.Amount = _viewModel.Amount;
                _contract.StartDate = DateTime.SpecifyKind(_viewModel.StartDate, DateTimeKind.Utc);
                _contract.EndDate = DateTime.SpecifyKind(_viewModel.EndDate, DateTimeKind.Utc);
                _contract.Status = _viewModel.Status;
                _contract.ClientId = _viewModel.SelectedClientId.Value;
                _contract.EmployeeId = _viewModel.SelectedEmployeeId.Value;
                _contract.BranchId = _viewModel.SelectedBranchId.Value;
                _contract.TypeId = _viewModel.SelectedDepositTypeId.Value;

                // Синхронизируем коллекции
                SyncDocuments();
                SyncOperations();

                // Сохраняем
                await _depositService.UpdateAsync(_contract);

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

        private void SyncDocuments()
        {
            var docsToRemove = _contract.Documents
                .Where(d => !_viewModel.Documents.Any(vd => vd.DocumentId == d.DocumentId))
                .ToList();

            foreach (var doc in docsToRemove)
                _contract.Documents.Remove(doc);

            foreach (var viewModelDoc in _viewModel.Documents)
            {
                var existingDoc = _contract.Documents
                    .FirstOrDefault(d => d.DocumentId == viewModelDoc.DocumentId);

                if (existingDoc != null)
                {
                    existingDoc.DocumentType = viewModelDoc.DocumentType;
                    existingDoc.FilePath = viewModelDoc.FilePath;
                }
                else
                {
                    viewModelDoc.ContractId = _contract.ContractId;
                    _contract.Documents.Add(viewModelDoc);
                }
            }
        }

        private void SyncOperations()
        {
            var opsToRemove = _contract.DepositOperations
                .Where(o => !_viewModel.DepositOperations.Any(vo => vo.OperationId == o.OperationId))
                .ToList();

            foreach (var op in opsToRemove)
                _contract.DepositOperations.Remove(op);

            foreach (var viewModelOp in _viewModel.DepositOperations)
            {
                var existingOp = _contract.DepositOperations
                    .FirstOrDefault(o => o.OperationId == viewModelOp.OperationId);

                if (existingOp != null)
                {
                    existingOp.OperationType = viewModelOp.OperationType;
                    existingOp.Amount = viewModelOp.Amount;
                    existingOp.OperationDate = DateTime.SpecifyKind(viewModelOp.OperationDate, DateTimeKind.Utc);
                }
                else
                {
                    viewModelOp.ContractId = _contract.ContractId;
                    viewModelOp.OperationDate = DateTime.SpecifyKind(viewModelOp.OperationDate, DateTimeKind.Utc);
                    _contract.DepositOperations.Add(viewModelOp);
                }
            }
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            if (_contract == null || !_canEdit) return;

            _viewModel.Documents.Add(new Document
            {
                DocumentId = 0,
                DocumentType = "Новый документ",
                FilePath = "",
                ContractId = _contract.ContractId
            });
        }

        private void RemoveDocument_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;

            if (DocumentsDataGrid.SelectedItem is Document selectedDoc)
            {
                if (MessageBox.Show("Удалить документ?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _viewModel.Documents.Remove(selectedDoc);
                }
            }
        }

        private void AddOperation_Click(object sender, RoutedEventArgs e)
        {
            if (_contract == null || !_canEdit) return;

            _viewModel.DepositOperations.Add(new DepositOperation
            {
                OperationId = 0,
                OperationType = "Пополнение",
                Amount = 0,
                OperationDate = DateTime.Now,
                ContractId = _contract.ContractId
            });
        }

        private void RemoveOperation_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;

            if (OperationsDataGrid.SelectedItem is DepositOperation selectedOp)
            {
                if (MessageBox.Show("Удалить операцию?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _viewModel.DepositOperations.Remove(selectedOp);
                }
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
