using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Model.enums;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;
using PIC_CURSACH.View.entityDetails;

namespace PIC_CURSACH.ViewModel.controls;

public partial class DepositContractsTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<DepositContract> depositContracts = new();
    [ObservableProperty] private DepositContract? selectedDepositContract;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IDepositContractService _depositContractService;
    private readonly IClientService _clientService;
    private readonly IDepositTypeService _depositTypeService;
    private readonly IEmployeeService _employeeService;
    private readonly IBranchService _branchService;

    public DepositContractsTableViewModel(
        IDepositContractService depositContractService,
        IClientService clientService,
        IDepositTypeService depositTypeService,
        IEmployeeService employeeService,
        IBranchService branchService)
    {
        _depositContractService = depositContractService;
        _clientService = clientService;
        _depositTypeService = depositTypeService;
        _employeeService = employeeService;
        _branchService = branchService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _depositContractService.GetAllAsync();
        DepositContracts = new ObservableCollection<DepositContract>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        // Загружаем данные для ComboBox
        var clients = await _clientService.GetAllAsync();
        var depositTypes = await _depositTypeService.GetAllAsync();
        var employees = await _employeeService.GetAllAsync();
        var branches = await _branchService.GetAllAsync();

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Клиент",
                PropertyName = "ClientId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = clients.Select(c => new ComboBoxItem
                {
                    DisplayName = $"{c.FirstName} {c.LastName} ({c.Passport})",
                    Value = c.ClientId.ToString()
                }).ToList()
            },
            new()
            {
                Label = "Тип депозита",
                PropertyName = "TypeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = depositTypes.Select(t => new ComboBoxItem
                {
                    DisplayName = $"{t.Name} ({t.InterestRate}%)",
                    Value = t.TypeId.ToString()
                }).ToList()
            },
            new()
            {
                Label = "Сотрудник",
                PropertyName = "EmployeeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = employees.Select(e => new ComboBoxItem
                {
                    DisplayName = $"{e.FirstName} {e.LastName} ({e.Position})",
                    Value = e.EmployeeId.ToString()
                }).ToList()
            },
            new()
            {
                Label = "Филиал",
                PropertyName = "BranchId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = branches.Select(b => new ComboBoxItem
                {
                    DisplayName = b.Name,
                    Value = b.BranchId.ToString()
                }).ToList()
            },
            new()
            {
                Label = "Сумма",
                PropertyName = "Amount",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Введите сумму депозита"
            },
            new()
            {
                Label = "Дата начала",
                PropertyName = "StartDate",
                FieldType = FormFieldType.DatePicker,
                IsRequired = true,
                Value = DateTime.Now.ToString("yyyy-MM-dd")
            },
            new()
            {
                Label = "Дата окончания",
                PropertyName = "EndDate",
                FieldType = FormFieldType.DatePicker,
                IsRequired = true,
                Value = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")
            },
            new()
            {
                Label = "Статус",
                PropertyName = "Status",
                FieldType = FormFieldType.Text,
                Value = "ACTIVE",
                Placeholder = "ACTIVE, CLOSED, EXPIRED"
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление депозитного договора", "Добавить", fields, SaveNewDepositContractAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedDepositContract == null) return;

        var clients = await _clientService.GetAllAsync();
        var depositTypes = await _depositTypeService.GetAllAsync();
        var employees = await _employeeService.GetAllAsync();
        var branches = await _branchService.GetAllAsync();
        var dc = SelectedDepositContract;

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Клиент",
                PropertyName = "ClientId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = clients.Select(c => new ComboBoxItem
                {
                    DisplayName = $"{c.FirstName} {c.LastName} ({c.Passport})",
                    Value = c.ClientId.ToString()
                }).ToList(),
                SelectedComboBoxValue = dc.ClientId.ToString()
            },
            new()
            {
                Label = "Тип депозита",
                PropertyName = "TypeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = depositTypes.Select(t => new ComboBoxItem
                {
                    DisplayName = $"{t.Name} ({t.InterestRate}%)",
                    Value = t.TypeId.ToString()
                }).ToList(),
                SelectedComboBoxValue = dc.TypeId.ToString()
            },
            new()
            {
                Label = "Сотрудник",
                PropertyName = "EmployeeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = employees.Select(e => new ComboBoxItem
                {
                    DisplayName = $"{e.FirstName} {e.LastName} ({e.Position})",
                    Value = e.EmployeeId.ToString()
                }).ToList(),
                SelectedComboBoxValue = dc.EmployeeId.ToString()
            },
            new()
            {
                Label = "Филиал",
                PropertyName = "BranchId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = branches.Select(b => new ComboBoxItem
                {
                    DisplayName = b.Name,
                    Value = b.BranchId.ToString()
                }).ToList(),
                SelectedComboBoxValue = dc.BranchId.ToString()
            },
            new()
            {
                Label = "Сумма",
                PropertyName = "Amount",
                FieldType = FormFieldType.Text,
                Value = dc.Amount.ToString()
            },
            new()
            {
                Label = "Дата начала",
                PropertyName = "StartDate",
                FieldType = FormFieldType.DatePicker,
                Value = dc.StartDate.ToString("yyyy-MM-dd")
            },
            new()
            {
                Label = "Дата окончания",
                PropertyName = "EndDate",
                FieldType = FormFieldType.DatePicker,
                Value = dc.EndDate.ToString("yyyy-MM-dd")
            },
            new()
            {
                Label = "Статус",
                PropertyName = "Status",
                FieldType = FormFieldType.Text,
                Value = dc.Status
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование депозитного договора", "Сохранить", fields,
            values => SaveExistingDepositContractAsync(dc.ContractId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteDepositContract()
    {
        if (SelectedDepositContract == null) return;

        var result = MessageBox.Show($"Удалить договор #{SelectedDepositContract.ContractId}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _depositContractService.DeleteAsync(SelectedDepositContract.ContractId);
                DepositContracts.Remove(SelectedDepositContract);
                MessageBox.Show("Договор успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public void OpenDepositContractDetails()
    {
        if (SelectedDepositContract == null) return;

        var detailsWindow = new DepositContractDetailsWindow
        {
            DataContext = SelectedDepositContract
        };
        detailsWindow.ShowDialog();
    }

    private async Task<bool> SaveNewDepositContractAsync(Dictionary<string, string> values)
    {
        try
        {
            var contract = new DepositContract
            {
                ClientId = int.Parse(values["ClientId"]),
                TypeId = int.Parse(values["TypeId"]),
                EmployeeId = int.Parse(values["EmployeeId"]),
                BranchId = int.Parse(values["BranchId"]),
                Amount = decimal.Parse(values["Amount"]),
                StartDate = DateTime.SpecifyKind(DateTime.Parse(values["StartDate"]), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(DateTime.Parse(values["EndDate"]), DateTimeKind.Utc),
                Status = values["Status"]
            };

            // Валидация
            if (contract.Amount <= 0)
            {
                MessageBox.Show("Сумма должна быть больше 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (contract.EndDate <= contract.StartDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            await _depositContractService.AddAsync(contract);
            MessageBox.Show("Депозитный договор успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (FormatException)
        {
            MessageBox.Show("Проверьте корректность введенных данных!", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingDepositContractAsync(int contractId, Dictionary<string, string> values)
    {
        try
        {
            var contract = await _depositContractService.GetByIdAsync(contractId);
            if (contract == null)
            {
                MessageBox.Show("Договор не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            contract.ClientId = int.Parse(values["ClientId"]);
            contract.TypeId = int.Parse(values["TypeId"]);
            contract.EmployeeId = int.Parse(values["EmployeeId"]);
            contract.BranchId = int.Parse(values["BranchId"]);
            contract.Amount = decimal.Parse(values["Amount"]);
            contract.StartDate = DateTime.SpecifyKind(DateTime.Parse(values["StartDate"]), DateTimeKind.Utc);
            contract.EndDate = DateTime.SpecifyKind(DateTime.Parse(values["EndDate"]), DateTimeKind.Utc);
            contract.Status = values["Status"];

            // Валидация
            if (contract.Amount <= 0)
            {
                MessageBox.Show("Сумма должна быть больше 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (contract.EndDate <= contract.StartDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            await _depositContractService.UpdateAsync(contract);
            MessageBox.Show("Депозитный договор успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (FormatException)
        {
            MessageBox.Show("Проверьте корректность введенных данных!", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}