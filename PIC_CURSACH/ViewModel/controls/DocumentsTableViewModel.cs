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

public partial class DocumentsTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Document> documents = new();
    [ObservableProperty] private Document? selectedDocument;
    [ObservableProperty] private bool canAdd = true;
    [ObservableProperty] private bool canEdit = true;
    [ObservableProperty] private bool canDelete = true;

    private readonly IDocumentService _documentService;
    private readonly IDepositContractService _depositContractService;

    public DocumentsTableViewModel(
        IDocumentService documentService,
        IDepositContractService depositContractService)
    {
        _documentService = documentService;
        _depositContractService = depositContractService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _documentService.GetAllAsync();
        Documents = new ObservableCollection<Document>(data);
    }

    [RelayCommand]
    private async Task ShowAddForm()
    {
        var contracts = await _depositContractService.GetAllAsync();

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор (необязательно)",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                IsRequired = false,
                ComboBoxItems = contracts.Select(c => new ComboBoxItem 
                { 
                    DisplayName = $"Договор #{c.ContractId} - {c.Amount}₽", 
                    Value = c.ContractId.ToString() 
                }).ToList()
            },
            new() 
            { 
                Label = "Тип документа", 
                PropertyName = "DocumentType", 
                FieldType = FormFieldType.Text, 
                IsRequired = true,
                Placeholder = "Договор, Паспорт, Заявление, Акт"
            },
            new() 
            { 
                Label = "Путь к файлу", 
                PropertyName = "FilePath", 
                FieldType = FormFieldType.Text,
                Placeholder = "/documents/contract_123.pdf"
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Добавление документа", "Добавить", fields, SaveNewDocumentAsync);
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task ShowEditForm()
    {
        if (SelectedDocument == null) return;

        var contracts = await _depositContractService.GetAllAsync();
        var doc = SelectedDocument;

        var fields = new List<FormField>
        {
            new()
            {
                Label = "Договор (необязательно)",
                PropertyName = "ContractId",
                FieldType = FormFieldType.ComboBox,
                ComboBoxItems = contracts.Select(c => new ComboBoxItem 
                { 
                    DisplayName = $"Договор #{c.ContractId} - {c.Amount}₽", 
                    Value = c.ContractId.ToString() 
                }).ToList(),
                SelectedComboBoxValue = doc.ContractId?.ToString()
            },
            new() 
            { 
                Label = "Тип документа", 
                PropertyName = "DocumentType", 
                FieldType = FormFieldType.Text, 
                Value = doc.DocumentType 
            },
            new() 
            { 
                Label = "Путь к файлу", 
                PropertyName = "FilePath", 
                FieldType = FormFieldType.Text, 
                Value = doc.FilePath ?? "" 
            }
        };

        var form = new UniversalEditForm();
        var viewModel = new UniversalEditFormViewModel(form, "Редактирование документа", "Сохранить", fields,
            values => SaveExistingDocumentAsync(doc.DocumentId, values));
        form.DataContext = viewModel;

        if (form.ShowDialog() == true && viewModel.DialogResult)
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteDocument()
    {
        if (SelectedDocument == null) return;

        var result = MessageBox.Show($"Удалить документ #{SelectedDocument.DocumentId} ({SelectedDocument.DocumentType})?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _documentService.DeleteAsync(SelectedDocument.DocumentId);
            Documents.Remove(SelectedDocument);
            MessageBox.Show("Документ успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async Task<bool> SaveNewDocumentAsync(Dictionary<string, string> values)
    {
        try
        {
            var document = new Document
            {
                ContractId = string.IsNullOrEmpty(values.GetValueOrDefault("ContractId")) 
                    ? null 
                    : int.Parse(values["ContractId"]),
                DocumentType = values["DocumentType"],
                FilePath = string.IsNullOrEmpty(values.GetValueOrDefault("FilePath")) 
                    ? null 
                    : values["FilePath"]
            };

            await _documentService.AddAsync(document);
            MessageBox.Show("Документ успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private async Task<bool> SaveExistingDocumentAsync(int documentId, Dictionary<string, string> values)
    {
        try
        {
            var document = await _documentService.GetByIdAsync(documentId);
            if (document == null)
            {
                MessageBox.Show("Документ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            document.ContractId = string.IsNullOrEmpty(values.GetValueOrDefault("ContractId")) 
                ? null 
                : int.Parse(values["ContractId"]);
            document.DocumentType = values["DocumentType"];
            document.FilePath = string.IsNullOrEmpty(values.GetValueOrDefault("FilePath")) 
                ? null 
                : values["FilePath"];

            await _documentService.UpdateAsync(document);
            MessageBox.Show("Документ успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}
