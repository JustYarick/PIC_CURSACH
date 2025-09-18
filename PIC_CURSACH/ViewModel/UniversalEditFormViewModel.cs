using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PIC_CURSACH.Model.core;

namespace PIC_CURSACH.ViewModel;

public partial class UniversalEditFormViewModel : ObservableObject
{
    private readonly Window _window;
    private readonly Func<Dictionary<string, string>, Task<bool>>? _saveAction;

    [ObservableProperty]
    private ObservableCollection<FormField> fields = new();

    [ObservableProperty]
    private string windowTitle = "Редактирование";

    [ObservableProperty]
    private string saveButtonText = "Сохранить";

    public bool DialogResult { get; private set; }

    public UniversalEditFormViewModel(
        Window window,
        string title,
        string saveButtonText,
        List<FormField> fields,
        Func<Dictionary<string, string>, Task<bool>>? saveAction = null)
    {
        _window = window;
        _saveAction = saveAction;

        WindowTitle = title;
        SaveButtonText = saveButtonText;
        Fields = new ObservableCollection<FormField>(fields);
    }

    [RelayCommand]
    private async Task Save()
    {
        // Валидация обязательных полей
        var emptyRequiredFields = Fields
            .Where(f => f.IsRequired && string.IsNullOrWhiteSpace(f.Value))
            .ToList();

        if (emptyRequiredFields.Any())
        {
            var fieldNames = string.Join(", ", emptyRequiredFields.Select(f => f.Label));
            MessageBox.Show($"Заполните обязательные поля: {fieldNames}", "Ошибка валидации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var values = Fields.ToDictionary(f => f.PropertyName, f => f.Value);

            bool success = true;
            if (_saveAction != null)
            {
                success = await _saveAction(values);
            }

            if (success)
            {
                DialogResult = true;
                _window.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        _window.Close();
    }
}