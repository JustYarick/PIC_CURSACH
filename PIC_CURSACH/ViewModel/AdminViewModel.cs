using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Model.enums;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;
using PIC_CURSACH.View.entityDetails;

namespace PIC_CURSACH.ViewModel;

public partial class AdminViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Client? selectedClient;

    [ObservableProperty]
    private ObservableCollection<Employee> employees = new();

    [ObservableProperty]
    private Employee? selectedEmployee;

    [ObservableProperty]
    private ObservableCollection<DepositOperation> depositOperations = new();

    [ObservableProperty]
    private DepositOperation? selectedDepositOperation;

    [ObservableProperty]
    private ObservableCollection<DepositType> depositTypes = new();

    [ObservableProperty]
    private DepositType? selectedDepositType;

    [ObservableProperty]
    private ObservableCollection<DepositContract> depositContracts = new();

    [ObservableProperty]
    private DepositContract? selectedDepositContract;

    private readonly IDepositContractService _depositContractService;
    private readonly IEmployeeService _employeeService;
    private readonly IDepositOperationService _depositOperationService;
    private readonly IDepositTypeService _depositTypeService;
    private readonly IClientService _clientService;

    public AdminViewModel()
    {
        _clientService = App.CurrentServiceConfigurator.Services.GetRequiredService<IClientService>();
        _employeeService = App.CurrentServiceConfigurator.Services.GetRequiredService<IEmployeeService>();
        _depositOperationService = App.CurrentServiceConfigurator.Services.GetRequiredService<IDepositOperationService>();
        _depositTypeService = App.CurrentServiceConfigurator.Services.GetRequiredService<IDepositTypeService>();
        _depositContractService = App.CurrentServiceConfigurator.Services.GetRequiredService<IDepositContractService>();
    }

    public async Task InitializeAsync()
    {
        await LoadClients();
        await LoadEmployees();
        await LoadDepositOperations();
        await LoadDepositTypes();
        await LoadDepositContracts();
    }

    [RelayCommand]
    private async Task LoadDepositContracts()
    {
        var data = await _depositContractService.GetAllAsync();
        DepositContracts = new ObservableCollection<DepositContract>(data);
    }

    [RelayCommand]
    private async Task LoadClients()
    {
        var data = await _clientService.GetAllAsync();
        Clients = new ObservableCollection<Client>(data);
    }

    [RelayCommand]
    private async Task LoadEmployees()
    {
        var data = await _employeeService.GetAllAsync();
        Employees = new ObservableCollection<Employee>(data);
    }

    [RelayCommand]
    private async Task LoadDepositOperations()
    {
        var data = await _depositOperationService.GetAllAsync();
        DepositOperations = new ObservableCollection<DepositOperation>(data);
    }

    [RelayCommand]
    private async Task LoadDepositTypes()
    {
        var data = await _depositTypeService.GetAllAsync();
        DepositTypes = new ObservableCollection<DepositType>(data);
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

    [RelayCommand] private async Task ShowAddDepositOperationForm()
    {
        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = DepositContracts.Select(c => new ComboBoxItem { DisplayName = c.ContractId.ToString(), Value = c.ContractId.ToString() }).ToList()
            },
            new() { Label = "Тип операции", PropertyName = "OperationType", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Сумма", PropertyName = "Amount", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Дата", PropertyName = "OperationDate", FieldType = FormFieldType.DatePicker, IsRequired = true }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Добавить операцию", "Добавить", fields, SaveNewDepositOperationAsync);
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositOperations();
    }

    [RelayCommand] private async Task ShowAddDepositTypeForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "Название", PropertyName = "Name", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Процент", PropertyName = "InterestRate", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Мин. сумма", PropertyName = "MinAmount", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Срок (дней)", PropertyName = "TermDays", FieldType = FormFieldType.Text, IsRequired = true }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Добавить тип депозита", "Добавить", fields, SaveNewDepositTypeAsync);
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositTypes();
    }

    [RelayCommand]
    private async Task ShowAddEmployeeForm()
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
            await LoadEmployees();
    }

    [RelayCommand] private async Task ShowAddDepositContractForm()
    {
        var fields = new List<FormField>
        {
            new()
            {
                Label = "Клиент",
                PropertyName = "ClientId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = Clients.Select(c => new ComboBoxItem { DisplayName = $"{c.FirstName} {c.LastName}", Value = c.ClientId.ToString() }).ToList()
            },
            new()
            {
                Label = "Тип депозита",
                PropertyName = "TypeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = DepositTypes.Select(t => new ComboBoxItem { DisplayName = t.Name, Value = t.TypeId.ToString() }).ToList()
            },
            new()
            {
                Label = "Сотрудник",
                PropertyName = "EmployeeId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = Employees.Select(e => new ComboBoxItem { DisplayName = $"{e.FirstName} {e.LastName}", Value = e.EmployeeId.ToString() }).ToList()
            },
            new()
            {
                Label = "Филиал",
                PropertyName = "BranchId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = true,
                ComboBoxItems = (await App.CurrentServiceConfigurator.Services.GetRequiredService<IBranchService>().GetAllAsync())
                    .Select(b => new ComboBoxItem { DisplayName = b.Name, Value = b.BranchId.ToString() }).ToList()
            },
            new() { Label = "Сумма", PropertyName = "Amount", FieldType = FormFieldType.Text, IsRequired = true },
            new() { Label = "Дата начала", PropertyName = "StartDate", FieldType = FormFieldType.DatePicker, IsRequired = true },
            new() { Label = "Дата окончания", PropertyName = "EndDate", FieldType = FormFieldType.DatePicker, IsRequired = true },
            new() { Label = "Статус", PropertyName = "Status", FieldType = FormFieldType.Text, Value = "ACTIVE" }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Добавить договор", "Добавить", fields, SaveNewDepositContractAsync);
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositContracts();
    }

    [RelayCommand] private async Task ShowEditDepositOperationForm()
    {
        if (SelectedDepositOperation == null) return;
        var op = SelectedDepositOperation;
        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = DepositContracts.Select(c => new ComboBoxItem { DisplayName = c.ContractId.ToString(), Value = c.ContractId.ToString() }).ToList(),
                SelectedComboBoxValue = op.ContractId.ToString()
            },
            new() { Label = "Тип операции", PropertyName = "OperationType", FieldType = FormFieldType.Text, Value = op.OperationType },
            new() { Label = "Сумма", PropertyName = "Amount", FieldType = FormFieldType.Text, Value = op.Amount.ToString() },
            new() { Label = "Дата", PropertyName = "OperationDate", FieldType = FormFieldType.DatePicker, Value = op.OperationDate.ToString("yyyy-MM-dd") }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Редактировать операцию", "Сохранить", fields,
            values => SaveExistingDepositOperationAsync(op.OperationId, values));
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositOperations();
    }

    [RelayCommand]
    private async Task ShowEditDepositTypeForm()
    {
        if (SelectedDepositType == null) return;
        var dt = SelectedDepositType;
        var fields = new List<FormField>
        {
            new() { Label = "Название", PropertyName = "Name", FieldType = FormFieldType.Text, Value = dt.Name },
            new() { Label = "Процент", PropertyName = "InterestRate", FieldType = FormFieldType.Text, Value = dt.InterestRate.ToString() },
            new() { Label = "Мин. сумма", PropertyName = "MinAmount", FieldType = FormFieldType.Text, Value = dt.MinAmount.ToString() },
            new() { Label = "Срок (дней)", PropertyName = "TermDays", FieldType = FormFieldType.Text, Value = dt.TermDays.ToString() }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Редактировать тип депозита", "Сохранить", fields,
            values => SaveExistingDepositTypeAsync(dt.TypeId, values));
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositTypes();
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

        if (form.ShowDialog() == false && viewModel.DialogResult)
            await LoadClients();
    }

    [RelayCommand]
    private async Task ShowEditDepositContractForm()
    {
        if (SelectedDepositContract == null) return;
        var dc = SelectedDepositContract;
        var fields = new List<FormField>
        {
            new()
            {
                Label = "Клиент",
                PropertyName = "ClientId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = Clients.Select(c => new ComboBoxItem { DisplayName = $"{c.FirstName} {c.LastName}", Value = c.ClientId.ToString() }).ToList(),
                SelectedComboBoxValue = dc.ClientId.ToString()
            },
            new()
            {
                Label = "Тип депозита",
                PropertyName = "TypeId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = DepositTypes.Select(t => new ComboBoxItem { DisplayName = t.Name, Value = t.TypeId.ToString() }).ToList(),
                SelectedComboBoxValue = dc.TypeId.ToString()
            },
            new()
            {
                Label = "Сотрудник",
                PropertyName = "EmployeeId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = Employees.Select(e => new ComboBoxItem { DisplayName = $"{e.FirstName} {e.LastName}", Value = e.EmployeeId.ToString() }).ToList(),
                SelectedComboBoxValue = dc.EmployeeId.ToString()
            },
            new()
            {
                Label = "Филиал",
                PropertyName = "BranchId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = (await App.CurrentServiceConfigurator.Services.GetRequiredService<IBranchService>().GetAllAsync())
                    .Select(b => new ComboBoxItem { DisplayName = b.Name, Value = b.BranchId.ToString() }).ToList(),
                SelectedComboBoxValue = dc.BranchId.ToString()
            },
            new() { Label = "Сумма", PropertyName = "Amount", FieldType = FormFieldType.Text, Value = dc.Amount.ToString() },
            new() { Label = "Дата начала", PropertyName = "StartDate", FieldType = FormFieldType.DatePicker, Value = dc.StartDate.ToString("yyyy-MM-dd") },
            new() { Label = "Дата окончания", PropertyName = "EndDate", FieldType = FormFieldType.DatePicker, Value = dc.EndDate.ToString("yyyy-MM-dd") },
            new() { Label = "Статус", PropertyName = "Status", FieldType = FormFieldType.Text, Value = dc.Status }
        };
        var form = new UniversalEditForm();
        var vm = new UniversalEditFormViewModel(form, "Редактировать договор", "Сохранить", fields,
            values => SaveExistingDepositContractAsync(dc.ContractId, values));
        form.DataContext = vm;
        if (form.ShowDialog() == true && vm.DialogResult)
            await LoadDepositContracts();
    }

    [RelayCommand]
    private async Task ShowEditEmployeeForm()
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
            await LoadEmployees();
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
            Clients.Remove(SelectedClient);
            MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteDepositOperation()
    {
        if (SelectedDepositOperation == null) return;

        var result = MessageBox.Show($"Удалить операцию {SelectedDepositOperation.OperationId}?", "Подтвердить", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            await _depositOperationService.DeleteAsync(SelectedDepositOperation.OperationId);
            DepositOperations.Remove(SelectedDepositOperation);
            MessageBox.Show("Операция удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteDepositType()
    {
        if (SelectedDepositType == null) return;

        var result = MessageBox.Show($"Удалить тип {SelectedDepositType.Name}?", "Подтвердить", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            await _depositTypeService.DeleteAsync(SelectedDepositType.TypeId);
            DepositTypes.Remove(SelectedDepositType);
            MessageBox.Show("Тип депозита удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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

    [RelayCommand]
    private async Task DeleteDepositContract()
    {
        if (SelectedDepositContract == null) return;

        var result = MessageBox.Show($"Удалить договор {SelectedDepositContract.ContractId}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _depositContractService.DeleteAsync(SelectedDepositContract.ContractId);
            DepositContracts.Remove(SelectedDepositContract);
            MessageBox.Show("Договор успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Метод сохранения нового клиента
    private async Task<bool> SaveNewClientAsync(Dictionary<string, string> values)
    {
        try
        {
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
            Clients.Add(client);
            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении клиента: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveNewDepositOperationAsync(Dictionary<string, string> values)
    {
        try
        {
            var op = new DepositOperation
            {
                ContractId = int.Parse(values["ContractId"]),
                OperationType = values["OperationType"],
                Amount = decimal.Parse(values["Amount"]),
                OperationDate = DateTime.SpecifyKind(DateTime.Parse(values["OperationDate"]), DateTimeKind.Utc)
            };
            await _depositOperationService.AddAsync(op);
            DepositOperations.Add(op);
            MessageBox.Show("Операция успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveNewDepositTypeAsync(Dictionary<string, string> values)
    {
        try
        {
            var dt = new DepositType
            {
                Name = values["Name"],
                InterestRate = decimal.Parse(values["InterestRate"]),
                MinAmount = decimal.Parse(values["MinAmount"]),
                TermDays = int.Parse(values["TermDays"])
            };
            await _depositTypeService.AddAsync(dt);
            DepositTypes.Add(dt);
            MessageBox.Show("Тип депозита успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
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
            Employees.Add(employee);
            MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveNewDepositContractAsync(Dictionary<string, string> values)
    {
        try
        {
            var dc = new DepositContract
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
            await _depositContractService.AddAsync(dc);
            DepositContracts.Add(dc);
            MessageBox.Show("Договор успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            client.Phone = string.IsNullOrEmpty(values.GetValueOrDefault("Phone")) ? null : values["Phone"];
            client.Email = string.IsNullOrEmpty(values.GetValueOrDefault("Email")) ? null : values["Email"];

            await _clientService.UpdateAsync(client);
            MessageBox.Show("Клиент успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            // Обновляем в коллекции (вызывается PropertyChanged у объекта)
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingDepositContractAsync(int id, Dictionary<string, string> values)
    {
        try
        {
            var dc = await _depositContractService.GetByIdAsync(id);
            if (dc == null) return false;
            dc.ClientId = int.Parse(values["ClientId"]);
            dc.TypeId = int.Parse(values["TypeId"]);
            dc.EmployeeId = int.Parse(values["EmployeeId"]);
            dc.BranchId = int.Parse(values["BranchId"]);
            dc.Amount = decimal.Parse(values["Amount"]);
            dc.StartDate = DateTime.SpecifyKind(DateTime.Parse(values["StartDate"]), DateTimeKind.Utc);
            dc.EndDate = DateTime.SpecifyKind(DateTime.Parse(values["EndDate"]), DateTimeKind.Utc);
            dc.Status = values["Status"];
            await _depositContractService.UpdateAsync(dc);
            MessageBox.Show("Договор успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            // Обновление коллекции произойдет автоматически если объект уведомляет об изменениях
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

    private async Task<bool> SaveExistingDepositOperationAsync(int id, Dictionary<string, string> values)
    {
        try
        {
            var op = await _depositOperationService.GetByIdAsync(id);
            if (op == null) return false;
            op.ContractId = int.Parse(values["ContractId"]);
            op.OperationType = values["OperationType"];
            op.Amount = decimal.Parse(values["Amount"]);
            op.OperationDate = DateTime.SpecifyKind(DateTime.Parse(values["OperationDate"]), DateTimeKind.Utc);
            await _depositOperationService.UpdateAsync(op);
            MessageBox.Show("Операция успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingDepositTypeAsync(int id, Dictionary<string, string> values)
    {
        try
        {
            var dt = await _depositTypeService.GetByIdAsync(id);
            if (dt == null) return false;
            dt.Name = values["Name"];
            dt.InterestRate = decimal.Parse(values["InterestRate"]);
            dt.MinAmount = decimal.Parse(values["MinAmount"]);
            dt.TermDays = int.Parse(values["TermDays"]);
            await _depositTypeService.UpdateAsync(dt);
            MessageBox.Show("Тип депозита успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public void OpenClientDetails()
    {
        if (SelectedClient == null)
            return;

        var detailsWindow = new ClientDetailsWindow
        {
            DataContext = SelectedClient
        };
        detailsWindow.ShowDialog();
    }

    public void OpenEmployeeDetails()
    {
        if (SelectedEmployee == null)
            return;

        var detailsWindow = new EmployeeDetailsWindow()
        {
            DataContext = SelectedEmployee
        };
        detailsWindow.ShowDialog();
    }

    public void OpenDepositTypeDetails()
    {
        if (SelectedDepositType == null)
            return;

        var detailsWindow = new DepositTypeDetailsWindow()
        {
            DataContext = SelectedDepositType
        };
        detailsWindow.ShowDialog();
    }

    public void OpenDepositContractDetails()
    {
        if (SelectedDepositContract == null)
            return;

        var detailsWindow = new DepositContractDetailsWindow()
        {
            DataContext = SelectedDepositContract
        };
        detailsWindow.ShowDialog();
    }
}