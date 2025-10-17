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

public partial class EmployeesTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Employee> employees = new();
    [ObservableProperty] private Employee? selectedEmployee;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IEmployeeService _employeeService;

    public EmployeesTableViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _employeeService.GetAllAsync();
        Employees = new ObservableCollection<Employee>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "Имя", Placeholder = "Введите имя сотрудника", IsRequired = true, PropertyName = "FirstName" },
            new() { Label = "Фамилия", Placeholder = "Введите фамилию сотрудника", IsRequired = true, PropertyName = "LastName" },
            new() { Label = "Должность", Placeholder = "Введите должность", IsRequired = true, PropertyName = "Position" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление сотрудника", "Добавить", fields, SaveNewEmployeeAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedEmployee == null) return;

        var fields = new List<FormField>
        {
            new() { Label = "Имя", Placeholder = "Введите имя сотрудника", IsRequired = true, PropertyName = "FirstName", Value = SelectedEmployee.FirstName },
            new() { Label = "Фамилия", Placeholder = "Введите фамилию сотрудника", IsRequired = true, PropertyName = "LastName", Value = SelectedEmployee.LastName },
            new() { Label = "Должность", Placeholder = "Введите должность", IsRequired = true, PropertyName = "Position", Value = SelectedEmployee.Position }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование сотрудника", "Сохранить", fields,
            values => SaveExistingEmployeeAsync(SelectedEmployee.EmployeeId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteEmployee()
    {
        if (SelectedEmployee == null) return;

        var result = MessageBox.Show($"Удалить сотрудника {SelectedEmployee.FirstName} {SelectedEmployee.LastName}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _employeeService.DeleteAsync(SelectedEmployee.EmployeeId);
            Employees.Remove(SelectedEmployee);
            MessageBox.Show("Сотрудник успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public void OpenEmployeeDetails()
    {
        if (SelectedEmployee == null) return;

        var detailsWindow = new EmployeeDetailsWindow(SelectedEmployee);
        detailsWindow.ShowDialog();
    }

    private async Task<bool> SaveNewEmployeeAsync(Dictionary<string, string> values)
    {
        try
        {
            var employee = new Employee
            {
                FirstName = values["FirstName"],
                LastName = values["LastName"],
                Position = values["Position"]
            };

            await _employeeService.AddAsync(employee);
            MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingEmployeeAsync(int employeeId, Dictionary<string, string> values)
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
            {
                MessageBox.Show("Сотрудник не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            employee.FirstName = values["FirstName"];
            employee.LastName = values["LastName"];
            employee.Position = values["Position"];

            await _employeeService.UpdateAsync(employee);
            MessageBox.Show("Сотрудник успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}