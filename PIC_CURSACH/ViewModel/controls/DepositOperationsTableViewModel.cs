using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Model.enums;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;

namespace PIC_CURSACH.ViewModel.controls;

public partial class DepositOperationsTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<DepositOperation> depositOperations = new();
    [ObservableProperty] private DepositOperation? selectedDepositOperation;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IDepositOperationService _depositOperationService;
    private readonly IDepositContractService _depositContractService;

    public DepositOperationsTableViewModel(
        IDepositOperationService depositOperationService,
        IDepositContractService depositContractService)
    {
        _depositOperationService = depositOperationService;
        _depositContractService = depositContractService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _depositOperationService.GetAllAsync();
        DepositOperations = new ObservableCollection<DepositOperation>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        // Загружаем договоры для ComboBox
        var contracts = await _depositContractService.GetAllAsync();

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = contracts.Select(c => new ComboBoxItem
                {
                    DisplayName = $"Договор #{c.ContractId}",
                    Value = c.ContractId.ToString()
                }).ToList()
            },
            new()
            {
                Label = "Тип операции",
                PropertyName = "OperationType",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Например: Пополнение, Снятие"
            },
            new()
            {
                Label = "Сумма",
                PropertyName = "Amount",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Введите сумму"
            },
            new()
            {
                Label = "Дата",
                PropertyName = "OperationDate",
                FieldType = FormFieldType.DatePicker,
                IsRequired = true,
                Value = DateTime.Now.ToString("yyyy-MM-dd")
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление операции", "Добавить", fields, SaveNewDepositOperationAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedDepositOperation == null) return;

        var contracts = await _depositContractService.GetAllAsync();
        var op = SelectedDepositOperation;

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = contracts.Select(c => new ComboBoxItem
                {
                    DisplayName = $"Договор #{c.ContractId}",
                    Value = c.ContractId.ToString()
                }).ToList(),
                SelectedComboBoxValue = op.ContractId.ToString()
            },
            new()
            {
                Label = "Тип операции",
                PropertyName = "OperationType",
                FieldType = FormFieldType.Text,
                Value = op.OperationType
            },
            new()
            {
                Label = "Сумма",
                PropertyName = "Amount",
                FieldType = FormFieldType.Text,
                Value = op.Amount.ToString()
            },
            new()
            {
                Label = "Дата",
                PropertyName = "OperationDate",
                FieldType = FormFieldType.DatePicker,
                Value = op.OperationDate.ToString("yyyy-MM-dd")
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование операции", "Сохранить", fields,
            values => SaveExistingDepositOperationAsync(op.OperationId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteDepositOperation()
    {
        if (SelectedDepositOperation == null) return;

        var result = MessageBox.Show($"Удалить операцию #{SelectedDepositOperation.OperationId}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _depositOperationService.DeleteAsync(SelectedDepositOperation.OperationId);
            DepositOperations.Remove(SelectedDepositOperation);
            MessageBox.Show("Операция успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async Task<bool> SaveNewDepositOperationAsync(Dictionary<string, string> values)
    {
        try
        {
            var operation = new DepositOperation
            {
                ContractId = int.Parse(values["ContractId"]),
                OperationType = values["OperationType"],
                Amount = decimal.Parse(values["Amount"]),
                OperationDate = DateTime.SpecifyKind(DateTime.Parse(values["OperationDate"]), DateTimeKind.Utc)
            };

            await _depositOperationService.AddAsync(operation);
            MessageBox.Show("Операция успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingDepositOperationAsync(int operationId, Dictionary<string, string> values)
    {
        try
        {
            var operation = await _depositOperationService.GetByIdAsync(operationId);
            if (operation == null)
            {
                MessageBox.Show("Операция не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            operation.ContractId = int.Parse(values["ContractId"]);
            operation.OperationType = values["OperationType"];
            operation.Amount = decimal.Parse(values["Amount"]);
            operation.OperationDate = DateTime.SpecifyKind(DateTime.Parse(values["OperationDate"]), DateTimeKind.Utc);

            await _depositOperationService.UpdateAsync(operation);
            MessageBox.Show("Операция успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}