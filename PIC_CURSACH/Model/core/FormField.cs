using System.ComponentModel;
using PIC_CURSACH.Model.enums;

namespace PIC_CURSACH.Model.core;

public class FormField : INotifyPropertyChanged
{
    private readonly Func<string>? _getter;
    private readonly Action<string>? _setter;
    private string _internalValue = string.Empty;

    public string Label { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = false;
    public bool IsEnabled { get; set; } = true;
    public string PropertyName { get; set; } = string.Empty;

    // Новый: тип поля
    public FormFieldType FieldType { get; set; } = FormFieldType.Text;

    // Для ComboBox
    public List<ComboBoxItem>? ComboBoxItems { get; set; }

    public string Value
    {
        get => _getter?.Invoke() ?? _internalValue;
        set
        {
            var oldValue = Value;
            if (_setter != null)
                _setter(value ?? string.Empty);
            else
                _internalValue = value ?? string.Empty;

            if (oldValue != value)
                OnPropertyChanged(nameof(Value));
        }
    }

    // Для ComboBox биндинг выбранного значения
    public string? SelectedComboBoxValue
    {
        get => Value;
        set
        {
            if (value != null)
                Value = value;
            OnPropertyChanged(nameof(SelectedComboBoxValue));
        }
    }

    public FormField(Func<string> getter, Action<string> setter)
    {
        _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        _setter = setter ?? throw new ArgumentNullException(nameof(setter));
    }

    public FormField() { }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}