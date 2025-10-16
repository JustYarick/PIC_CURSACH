using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.ViewModel.controls;

namespace PIC_CURSACH.View.controls;

public partial class DepositContractsTableControl : UserControl
{
    public static readonly DependencyProperty CanAddProperty =
        DependencyProperty.Register(nameof(CanAdd), typeof(bool),
            typeof(DepositContractsTableControl), new PropertyMetadata(true));

    public static readonly DependencyProperty CanEditProperty =
        DependencyProperty.Register(nameof(CanEdit), typeof(bool),
            typeof(DepositContractsTableControl), new PropertyMetadata(true));

    public static readonly DependencyProperty CanDeleteProperty =
        DependencyProperty.Register(nameof(CanDelete), typeof(bool),
            typeof(DepositContractsTableControl), new PropertyMetadata(true));

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

    public DepositContractsTableViewModel? ViewModel { get; private set; }
    private bool _isLoaded = false;

    public DepositContractsTableControl()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isLoaded) return;
        _isLoaded = true;

        var serviceProvider = App.CurrentServiceConfigurator?.Services;
        if (serviceProvider != null)
        {
            ViewModel = serviceProvider.GetRequiredService<DepositContractsTableViewModel>();
            DataContext = ViewModel;

            ViewModel.CanAdd = CanAdd;
            ViewModel.CanEdit = CanEdit;
            ViewModel.CanDelete = CanDelete;

            await ViewModel.LoadDataAsync();
        }
    }

    private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel?.OpenDepositContractDetails();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (ViewModel != null)
        {
            if (e.Property == CanAddProperty)
                ViewModel.CanAdd = (bool)e.NewValue;
            else if (e.Property == CanEditProperty)
                ViewModel.CanEdit = (bool)e.NewValue;
            else if (e.Property == CanDeleteProperty)
                ViewModel.CanDelete = (bool)e.NewValue;
        }
    }
}