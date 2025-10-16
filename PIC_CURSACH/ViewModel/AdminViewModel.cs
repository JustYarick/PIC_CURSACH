using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.View;

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

    [RelayCommand]
    private async Task ShowAddDepositOperationForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "ID договора", Placeholder = "Введите ID договора", IsRequired = true, PropertyName = "ContractId" },
            new() { Label = "Тип операции", Placeholder = "Введите тип операции", IsRequired = true, PropertyName = "OperationType" },
            new() { Label = "Сумма", Placeholder = "Введите сумму", IsRequired = true, PropertyName = "Amount" },
            new() { Label = "Дата операции", Placeholder = "Введите дату", IsRequired = true, PropertyName = "OperationDate" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление операции по депозиту", "Добавить", fields, SaveNewDepositOperationAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDepositOperations();
    }

    [RelayCommand]
    private async Task ShowAddDepositTypeForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "Название", Placeholder = "Введите название типа", IsRequired = true, PropertyName = "Name" },
            new() { Label = "Процент", Placeholder = "Введите процент", IsRequired = true, PropertyName = "InterestRate" },
            new() { Label = "Минимальная сумма", Placeholder = "Введите минимум", IsRequired = true, PropertyName = "MinAmount" },
            new() { Label = "Срок (дней)", Placeholder = "Введите срок", IsRequired = true, PropertyName = "TermDays" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление типа депозита", "Добавить", fields, SaveNewDepositTypeAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
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

    [RelayCommand]
    private async Task ShowAddDepositContractForm()
    {
        var fields = new List<FormField>
        {
            new() { Label = "ID клиента", Placeholder = "Введите ID клиента", IsRequired = true, PropertyName = "ClientId" },
            new() { Label = "ID типа депозита", Placeholder = "Введите ID типа депозита", IsRequired = true, PropertyName = "TypeId" },
            new() { Label = "ID сотрудника", Placeholder = "Введите ID сотрудника", IsRequired = true, PropertyName = "EmployeeId" },
            new() { Label = "ID филиала", Placeholder = "Введите ID филиала", IsRequired = true, PropertyName = "BranchId" },
            new() { Label = "Сумма", Placeholder = "Введите сумму", IsRequired = true, PropertyName = "Amount" },
            new() { Label = "Дата начала", Placeholder = "Введите дату начала", IsRequired = true, PropertyName = "StartDate" },
            new() { Label = "Дата окончания", Placeholder = "Введите дату окончания", IsRequired = true, PropertyName = "EndDate" },
            new() { Label = "Статус", Placeholder = "Введите статус", PropertyName = "Status", Value = "ACTIVE" }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление договора", "Добавить", fields, SaveNewDepositContractAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDepositContracts();
    }

    [RelayCommand]
    private async Task ShowEditDepositOperationForm()
    {
        if (SelectedDepositOperation == null) return;

        var fields = new List<FormField>
        {
            new() { Label = "ID договора", PropertyName = "ContractId", Value = SelectedDepositOperation.ContractId.ToString() },
            new() { Label = "Тип операции", PropertyName = "OperationType", Value = SelectedDepositOperation.OperationType },
            new() { Label = "Сумма", PropertyName = "Amount", Value = SelectedDepositOperation.Amount.ToString() },
            new() { Label = "Дата операции", PropertyName = "OperationDate", Value = SelectedDepositOperation.OperationDate.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование операции по депозиту", "Сохранить", fields,
            values => SaveExistingDepositOperationAsync(SelectedDepositOperation.OperationId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDepositOperations();
    }

    [RelayCommand]
    private async Task ShowEditDepositTypeForm()
    {
        if (SelectedDepositType == null) return;

        var fields = new List<FormField>
        {
            new() { Label = "Название", PropertyName = "Name", Value = SelectedDepositType.Name },
            new() { Label = "Процент", PropertyName = "InterestRate", Value = SelectedDepositType.InterestRate.ToString() },
            new() { Label = "Мин. сумма", PropertyName = "MinAmount", Value = SelectedDepositType.MinAmount.ToString() },
            new() { Label = "Срок (дней)", PropertyName = "TermDays", Value = SelectedDepositType.TermDays.ToString() }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование типа депозита", "Сохранить", fields,
            values => SaveExistingDepositTypeAsync(SelectedDepositType.TypeId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
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

        var fields = new List<FormField>
        {
            new() { Label = "ID клиента", IsRequired = true, PropertyName = "ClientId", Value = SelectedDepositContract.ClientId.ToString() },
            new() { Label = "ID типа депозита", IsRequired = true, PropertyName = "TypeId", Value = SelectedDepositContract.TypeId.ToString() },
            new() { Label = "ID сотрудника", IsRequired = true, PropertyName = "EmployeeId", Value = SelectedDepositContract.EmployeeId.ToString() },
            new() { Label = "ID филиала", IsRequired = true, PropertyName = "BranchId", Value = SelectedDepositContract.BranchId.ToString() },
            new() { Label = "Сумма", IsRequired = true, PropertyName = "Amount", Value = SelectedDepositContract.Amount.ToString() },
            new() { Label = "Дата начала", IsRequired = true, PropertyName = "StartDate", Value = SelectedDepositContract.StartDate.ToString("yyyy-MM-dd") },
            new() { Label = "Дата окончания", IsRequired = true, PropertyName = "EndDate", Value = SelectedDepositContract.EndDate.ToString("yyyy-MM-dd") },
            new() { Label = "Статус", PropertyName = "Status", Value = SelectedDepositContract.Status }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование договора", "Сохранить", fields,
            values => SaveExistingDepositContractAsync(SelectedDepositContract.ContractId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
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
            await LoadClients();
            MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteDepositOperation()
    {
        if (SelectedDepositOperation == null) return;

        var result = MessageBox.Show($"Удалить операцию с ID {SelectedDepositOperation.OperationId}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _depositOperationService.DeleteAsync(SelectedDepositOperation.OperationId);
            await LoadDepositOperations();
            MessageBox.Show("Операция удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteDepositType()
    {
        if (SelectedDepositType == null) return;

        var result = MessageBox.Show($"Удалить тип депозита {SelectedDepositType.Name}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _depositTypeService.DeleteAsync(SelectedDepositType.TypeId);
            await LoadDepositTypes();
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
            await LoadEmployees();
            MessageBox.Show("Сотрудник успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteDepositContract()
    {
        if (SelectedDepositContract == null) return;

        var result = MessageBox.Show($"Удалить депозитный договор {SelectedDepositContract.ContractId}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _depositContractService.DeleteAsync(SelectedDepositContract.ContractId);
            await LoadDepositContracts();
            MessageBox.Show("Договор успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Метод сохранения нового клиента
    private async Task<bool> SaveNewClientAsync(Dictionary<string, string> values)
    {
        try
        {
            // Проверка уникальности паспорта
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
            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadClients();
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
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
                OperationDate = DateTime.Parse(values["OperationDate"])
            };
            await _depositOperationService.AddAsync(operation);
            MessageBox.Show("Операция успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDepositOperations();
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
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
            await _depositTypeService.AddAsync(depositType);
            MessageBox.Show("Тип депозита успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDepositTypes();
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadEmployees();
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
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
                StartDate = DateTime.Parse(values["StartDate"]),
                EndDate = DateTime.Parse(values["EndDate"]),
                Status = values.ContainsKey("Status") ? values["Status"] : "ACTIVE"
            };

            await _depositContractService.AddAsync(contract);
            MessageBox.Show("Договор успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDepositContracts();
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Паспорт не обновляем - он заблокирован при редактировании
            client.Phone = string.IsNullOrEmpty(values.GetValueOrDefault("Phone")) ? null : values["Phone"];
            client.Email = string.IsNullOrEmpty(values.GetValueOrDefault("Email")) ? null : values["Email"];

            await _clientService.UpdateAsync(client);
            MessageBox.Show("Клиент успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            contract.StartDate = DateTime.Parse(values["StartDate"]);
            contract.EndDate = DateTime.Parse(values["EndDate"]);
            contract.Status = values.ContainsKey("Status") ? values["Status"] : contract.Status;

            await _depositContractService.UpdateAsync(contract);
            MessageBox.Show("Договор успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            operation.OperationDate = DateTime.Parse(values["OperationDate"]);

            await _depositOperationService.UpdateAsync(operation);
            MessageBox.Show("Операция успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            await _depositTypeService.UpdateAsync(depositType);
            MessageBox.Show("Тип депозита успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            MessageBox.Show($"Ошибка при добавлении операции: {err}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}