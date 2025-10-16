using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PIC_CURSACH.View.controls;

public partial class EntityTableControl : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
            typeof(EntityTableControl), new PropertyMetadata(null, OnItemsSourceChanged));

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object),
            typeof(EntityTableControl), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty AddCommandProperty =
        DependencyProperty.Register(nameof(AddCommand), typeof(ICommand),
            typeof(EntityTableControl), new PropertyMetadata(null, OnAddCommandChanged));

    public static readonly DependencyProperty EditCommandProperty =
        DependencyProperty.Register(nameof(EditCommand), typeof(ICommand),
            typeof(EntityTableControl), new PropertyMetadata(null, OnEditCommandChanged));

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand),
            typeof(EntityTableControl), new PropertyMetadata(null, OnDeleteCommandChanged));

    public static readonly DependencyProperty CanAddProperty =
        DependencyProperty.Register(nameof(CanAdd), typeof(bool),
            typeof(EntityTableControl), new PropertyMetadata(true, OnCanAddChanged));

    public static readonly DependencyProperty CanEditProperty =
        DependencyProperty.Register(nameof(CanEdit), typeof(bool),
            typeof(EntityTableControl), new PropertyMetadata(true, OnCanEditChanged));

    public static readonly DependencyProperty CanDeleteProperty =
        DependencyProperty.Register(nameof(CanDelete), typeof(bool),
            typeof(EntityTableControl), new PropertyMetadata(true, OnCanDeleteChanged));

    // Публичное свойство для доступа к колонкам DataGrid
    public ObservableCollection<DataGridColumn> Columns => DataGridControl.Columns;

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public ICommand AddCommand
    {
        get => (ICommand)GetValue(AddCommandProperty);
        set => SetValue(AddCommandProperty, value);
    }

    public ICommand EditCommand
    {
        get => (ICommand)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public bool CanAdd
    {
        get => (bool)GetValue(CanAddProperty);
        set => SetValue(CanAddProperty, value);
    }

    public bool CanEdit
    {
        get => (bool)GetValue(CanEditProperty);
        set => SetValue(CanEditProperty, value);
    }

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        set => SetValue(CanDeleteProperty, value);
    }

    public event MouseButtonEventHandler? RowDoubleClick;

    public EntityTableControl()
    {
        InitializeComponent();
        DataGridControl.MouseDoubleClick += (s, e) => RowDoubleClick?.Invoke(s, e);

        var binding = new Binding(nameof(SelectedItem))
        {
            Source = this,
            Mode = BindingMode.TwoWay
        };
        DataGridControl.SetBinding(DataGrid.SelectedItemProperty, binding);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.DataGridControl.ItemsSource = e.NewValue as IEnumerable;
    }

    private static void OnAddCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.AddButton.Command = e.NewValue as ICommand;
    }

    private static void OnEditCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.EditButton.Command = e.NewValue as ICommand;
    }

    private static void OnDeleteCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.DeleteButton.Command = e.NewValue as ICommand;
    }

    private static void OnCanAddChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.AddButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnCanEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.EditButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnCanDeleteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EntityTableControl control)
            control.DeleteButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }
}