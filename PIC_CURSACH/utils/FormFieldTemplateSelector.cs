using System.Windows;
using System.Windows.Controls;
using PIC_CURSACH.Model.core;
using PIC_CURSACH.Model.enums;

namespace PIC_CURSACH.utils;

public class FormFieldTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextFieldTemplate { get; set; }
    public DataTemplate? ComboBoxFieldTemplate { get; set; }
    public DataTemplate? DatePickerFieldTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is FormField field)
        {
            return field.FieldType switch
            {
                FormFieldType.Text => TextFieldTemplate,
                FormFieldType.ComboBox => ComboBoxFieldTemplate,
                FormFieldType.DatePicker => DatePickerFieldTemplate,
                _ => TextFieldTemplate
            };
        }
        return base.SelectTemplate(item, container);
    }
}