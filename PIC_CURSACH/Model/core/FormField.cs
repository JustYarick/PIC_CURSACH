using System.ComponentModel;

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

    public string Value
    {
        get
        {
            return _getter?.Invoke() ?? _internalValue;
        }
        set
        {
            var oldValue = Value;

            if (_setter != null)
            {
                _setter(value ?? string.Empty);
            }
            else
            {
                _internalValue = value ?? string.Empty;
            }

            if (oldValue != value)
            {
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public FormField(Func<string> getter, Action<string> setter)
    {
        _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        _setter = setter ?? throw new ArgumentNullException(nameof(setter));
    }

    public FormField()
    {
        _getter = null;
        _setter = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}