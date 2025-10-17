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

public partial class DepositTypesTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<DepositType> depositTypes = new();
    [ObservableProperty] private DepositType? selectedDepositType;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IDepositTypeService _depositTypeService;

    public DepositTypesTableViewModel(IDepositTypeService depositTypeService)
    {
        _depositTypeService = depositTypeService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _depositTypeService.GetAllAsync();
        DepositTypes = new ObservableCollection<DepositType>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        var fields = new List<FormField>
        {
            new()
            {
                Label = "Название",
                PropertyName = "Name",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Например: Стандартный, Премиум"
            },
            new()
            {
                Label = "Процентная ставка (%)",
                PropertyName = "InterestRate",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Например: 5.5"
            },
            new()
            {
                Label = "Минимальная сумма",
                PropertyName = "MinAmount",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Например: 10000"
            },
            new()
            {
                Label = "Срок (дней)",
                PropertyName = "TermDays",
                FieldType = FormFieldType.Text,
                IsRequired = true,
                Placeholder = "Например: 365"
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление типа депозита", "Добавить", fields, SaveNewDepositTypeAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedDepositType == null) return;

        var dt = SelectedDepositType;

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Название",
                PropertyName = "Name",
                FieldType = FormFieldType.Text,
                Value = dt.Name
            },
            new()
            {
                Label = "Процентная ставка (%)",
                PropertyName = "InterestRate",
                FieldType = FormFieldType.Text,
                Value = dt.InterestRate.ToString()
            },
            new()
            {
                Label = "Минимальная сумма",
                PropertyName = "MinAmount",
                FieldType = FormFieldType.Text,
                Value = dt.MinAmount.ToString()
            },
            new()
            {
                Label = "Срок (дней)",
                PropertyName = "TermDays",
                FieldType = FormFieldType.Text,
                Value = dt.TermDays.ToString()
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование типа депозита", "Сохранить", fields,
            values => SaveExistingDepositTypeAsync(dt.TypeId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteDepositType()
    {
        if (SelectedDepositType == null) return;

        var result = MessageBox.Show($"Удалить тип депозита '{SelectedDepositType.Name}'?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _depositTypeService.DeleteAsync(SelectedDepositType.TypeId);
                DepositTypes.Remove(SelectedDepositType);
                MessageBox.Show("Тип депозита успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public void OpenDepositTypeDetails()
    {
        if (SelectedDepositType == null) return;

        var detailsWindow = new DepositTypeDetailsWindow(SelectedDepositType);
        detailsWindow.ShowDialog();
    }

    private async Task<bool> SaveNewDepositTypeAsync(Dictionary<string, string> values)
    {
        try
        {
            var depositType = new DepositType
            {
                Name = values["Name"],
                InterestRate = decimal.Parse(values["InterestRate"]),
                MinAmount = decimal.Parse(values["MinAmount"]),
                TermDays = int.Parse(values["TermDays"])
            };

            // Валидация
            if (depositType.InterestRate < 0 || depositType.InterestRate > 100)
            {
                MessageBox.Show("Процентная ставка должна быть от 0 до 100!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (depositType.MinAmount <= 0)
            {
                MessageBox.Show("Минимальная сумма должна быть больше 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (depositType.TermDays <= 0)
            {
                MessageBox.Show("Срок должен быть больше 0 дней!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            await _depositTypeService.AddAsync(depositType);
            MessageBox.Show("Тип депозита успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (FormatException)
        {
            MessageBox.Show("Проверьте корректность введенных числовых значений!", "Ошибка",
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

    private async Task<bool> SaveExistingDepositTypeAsync(int typeId, Dictionary<string, string> values)
    {
        try
        {
            var depositType = await _depositTypeService.GetByIdAsync(typeId);
            if (depositType == null)
            {
                MessageBox.Show("Тип депозита не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            depositType.Name = values["Name"];
            depositType.InterestRate = decimal.Parse(values["InterestRate"]);
            depositType.MinAmount = decimal.Parse(values["MinAmount"]);
            depositType.TermDays = int.Parse(values["TermDays"]);

            // Валидация
            if (depositType.InterestRate < 0 || depositType.InterestRate > 100)
            {
                MessageBox.Show("Процентная ставка должна быть от 0 до 100!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (depositType.MinAmount <= 0)
            {
                MessageBox.Show("Минимальная сумма должна быть больше 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (depositType.TermDays <= 0)
            {
                MessageBox.Show("Срок должен быть больше 0 дней!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            await _depositTypeService.UpdateAsync(depositType);
            MessageBox.Show("Тип депозита успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (FormatException)
        {
            MessageBox.Show("Проверьте корректность введенных числовых значений!", "Ошибка",
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